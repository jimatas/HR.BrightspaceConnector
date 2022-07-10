using HR.BrightspaceConnector.Features.Common;
using HR.BrightspaceConnector.Features.Courses.Queries;
using HR.Common.Cqrs;
using HR.Common.Cqrs.Commands;
using HR.Common.Utilities;

namespace HR.BrightspaceConnector.Features.Courses.Commands
{
    public class ProcessCourseOfferings : ICommand
    {
        public ProcessCourseOfferings(int batchSize, bool isDeleteContext = false)
        {
            BatchSize = batchSize;
            IsDeleteContext = isDeleteContext;
        }

        public int BatchSize { get; }
        public bool IsDeleteContext { get; }
    }

    public class ProcessCourseOfferingsHandler : ICommandHandler<ProcessCourseOfferings>
    {
        private readonly IDispatcher dispatcher;
        private readonly ILogger logger;

        public ProcessCourseOfferingsHandler(IDispatcher dispatcher, ILogger<ProcessCourseOfferings> logger)
        {
            this.dispatcher = dispatcher;
            this.logger = logger;
        }

        public async Task HandleAsync(ProcessCourseOfferings command, CancellationToken cancellationToken)
        {
            int created = 0,
                updated = 0,
                deleted = 0;

            logger.LogInformation("Processing course offerings to {Action}.", command.IsDeleteContext ? "delete" : "create or update");

            for (var i = 0; i < command.BatchSize; i++)
            {
                var nextCourseOffering = await dispatcher.DispatchAsync(new GetNextCourseOffering(), cancellationToken).WithoutCapturingContext();
                if (nextCourseOffering is null ||
                    ((nextCourseOffering.IsToBeCreated() || nextCourseOffering.IsToBeUpdated()) && command.IsDeleteContext) ||
                    (nextCourseOffering.IsToBeDeleted() && !command.IsDeleteContext))
                {
                    logger.LogInformation("No more course offerings to {Action}. Ending batch run.", command.IsDeleteContext ? "delete" : "create or update");
                    break;
                }

                if (nextCourseOffering.IsToBeCreated())
                {
                    await dispatcher.DispatchAsync(new CreateCourseOffering(nextCourseOffering), cancellationToken).WithoutCapturingContext();
                    created++;
                }
                else if (nextCourseOffering.IsToBeUpdated())
                {
                    await dispatcher.DispatchAsync(new UpdateCourseOffering(nextCourseOffering), cancellationToken).WithoutCapturingContext();
                    updated++;
                }
                else if (nextCourseOffering.IsToBeDeleted())
                {
                    await dispatcher.DispatchAsync(new DeleteCourseOffering(nextCourseOffering), cancellationToken).WithoutCapturingContext();
                    deleted++;
                }
            }

            logger.LogInformation("Processed {Processed} course offering(s) in total.", created + updated + deleted);
            if (command.IsDeleteContext)
            {
                logger.LogDebug("Deleted {Deleted} course offering(s).", deleted);
            }
            else
            {
                logger.LogDebug("Created {Created} course offering(s).", created);
                logger.LogDebug("Updated {Updated} course offering(s).", updated);
            }
        }
    }
}
