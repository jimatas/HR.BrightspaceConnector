using HR.BrightspaceConnector.Features.Common;
using HR.BrightspaceConnector.Features.Enrollments.Queries;
using HR.Common.Cqrs;
using HR.Common.Cqrs.Commands;
using HR.Common.Utilities;

namespace HR.BrightspaceConnector.Features.Enrollments.Commands
{
    public class ProcessEnrollments : ICommand
    {
        public ProcessEnrollments(int batchSize, bool isDeleteContext = false)
        {
            BatchSize = batchSize;
            IsDeleteContext = isDeleteContext;
        }

        public int BatchSize { get; }
        public bool IsDeleteContext { get; }
    }

    public class ProcessEnrollmentsHandler : ICommandHandler<ProcessEnrollments>
    {
        private readonly IDispatcher dispatcher;
        private readonly ILogger logger;

        public ProcessEnrollmentsHandler(IDispatcher dispatcher, ILogger<ProcessEnrollments> logger)
        {
            this.dispatcher = dispatcher;
            this.logger = logger;
        }

        public async Task HandleAsync(ProcessEnrollments command, CancellationToken cancellationToken)
        {
            int created = 0,
                updated = 0,
                deleted = 0;

            logger.LogInformation("Processing enrollments to {Action}.", command.IsDeleteContext ? "delete" : "create or update");

            for (var i = 0; i < command.BatchSize; i++)
            {
                var nextEnrollment = await dispatcher.DispatchAsync(new GetNextEnrollment(), cancellationToken).WithoutCapturingContext();
                if (nextEnrollment is null ||
                    ((nextEnrollment.IsToBeCreated() || nextEnrollment.IsToBeUpdated()) && command.IsDeleteContext) ||
                    (nextEnrollment.IsToBeDeleted() && !command.IsDeleteContext))
                {
                    logger.LogInformation("No (more) enrollments to {Action}. Ending batch run.", command.IsDeleteContext ? "delete" : "create or update");
                    break;
                }

                if (nextEnrollment.IsToBeCreated())
                {
                    await dispatcher.DispatchAsync(new CreateEnrollment(nextEnrollment), cancellationToken).WithoutCapturingContext();
                    created++;
                }
                else if (nextEnrollment.IsToBeUpdated())
                {
                    await dispatcher.DispatchAsync(new UpdateEnrollment(nextEnrollment), cancellationToken).WithoutCapturingContext();
                    updated++;
                }
                else if (nextEnrollment.IsToBeDeleted())
                {
                    await dispatcher.DispatchAsync(new DeleteEnrollment(nextEnrollment), cancellationToken).WithoutCapturingContext();
                    deleted++;
                }
            }

            logger.LogInformation("Processed {Processed} enrollment(s) in total.", created + updated + deleted);
            if (command.IsDeleteContext)
            {
                logger.LogDebug("Deleted {Deleted} enrollment(s).", deleted);
            }
            else
            {
                logger.LogDebug("Created {Created} enrollment(s).", created);
                logger.LogDebug("Updated {Updated} enrollment(s).", updated);
            }
        }
    }
}
