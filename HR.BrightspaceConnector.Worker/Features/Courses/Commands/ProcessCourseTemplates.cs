using HR.BrightspaceConnector.Features.Common;
using HR.BrightspaceConnector.Features.Courses.Queries;
using HR.Common.Cqrs;
using HR.Common.Cqrs.Commands;
using HR.Common.Utilities;

namespace HR.BrightspaceConnector.Features.Courses.Commands
{
    public class ProcessCourseTemplates : ICommand
    {
        public ProcessCourseTemplates(int batchSize, bool isDeleteContext = false)
        {
            BatchSize = batchSize;
            IsDeleteContext = isDeleteContext;
        }

        public int BatchSize { get; }
        public bool IsDeleteContext { get; }
    }

    public class ProcessCourseTemplatesHandler : ICommandHandler<ProcessCourseTemplates>
    {
        private readonly IDispatcher dispatcher;
        private readonly ILogger logger;

        public ProcessCourseTemplatesHandler(IDispatcher dispatcher, ILogger<ProcessCourseTemplates> logger)
        {
            this.dispatcher = dispatcher;
            this.logger = logger;
        }

        public async Task HandleAsync(ProcessCourseTemplates command, CancellationToken cancellationToken)
        {
            int created = 0,
                updated = 0,
                deleted = 0;

            logger.LogInformation("Processing course templates to {Action}.", command.IsDeleteContext ? "delete" : "create or update");

            for (var i = 0; i < command.BatchSize; i++)
            {
                var nextCourseTemplate = await dispatcher.DispatchAsync(new GetNextCourseTemplate(), cancellationToken).WithoutCapturingContext();
                if (nextCourseTemplate is null ||
                    ((nextCourseTemplate.IsToBeCreated() || nextCourseTemplate.IsToBeUpdated()) && command.IsDeleteContext) ||
                    (nextCourseTemplate.IsToBeDeleted() && !command.IsDeleteContext))
                {
                    logger.LogInformation("No (more) course templates to {Action}. Ending batch run.", command.IsDeleteContext ? "delete" : "create or update");
                    break;
                }

                if (nextCourseTemplate.IsToBeCreated())
                {
                    await dispatcher.DispatchAsync(new CreateCourseTemplate(nextCourseTemplate), cancellationToken).WithoutCapturingContext();
                    created++;
                }
                else if (nextCourseTemplate.IsToBeUpdated())
                {
                    await dispatcher.DispatchAsync(new UpdateCourseTemplate(nextCourseTemplate), cancellationToken).WithoutCapturingContext();
                    updated++;
                }
                else if (nextCourseTemplate.IsToBeDeleted())
                {
                    await dispatcher.DispatchAsync(new DeleteCourseTemplate(nextCourseTemplate), cancellationToken).WithoutCapturingContext();
                    deleted++;
                }
            }

            logger.LogInformation("Processed {Processed} course template(s) in total.", created + updated + deleted);
            if (command.IsDeleteContext)
            {
                logger.LogDebug("Deleted {Deleted} course template(s).", deleted);
            }
            else
            {
                logger.LogDebug("Created {Created} course template(s).", created);
                logger.LogDebug("Updated {Updated} course template(s).", updated);
            }
        }
    }
}
