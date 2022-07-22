using HR.BrightspaceConnector.Features.Common;
using HR.BrightspaceConnector.Features.Sections.Queries;
using HR.Common.Cqrs;
using HR.Common.Cqrs.Commands;
using HR.Common.Utilities;

namespace HR.BrightspaceConnector.Features.Sections.Commands
{
    public class ProcessSections : ICommand
    {
        public ProcessSections(int batchSize, bool isDeleteContext = false)
        {
            BatchSize = batchSize;
            IsDeleteContext = isDeleteContext;
        }

        public int BatchSize { get; }
        public bool IsDeleteContext { get; }
    }

    public class ProcessSectionsHandler : ICommandHandler<ProcessSections>
    {
        private readonly IDispatcher dispatcher;
        private readonly ILogger logger;

        public ProcessSectionsHandler(IDispatcher dispatcher, ILogger<ProcessSections> logger)
        {
            this.dispatcher = dispatcher;
            this.logger = logger;
        }

        public async Task HandleAsync(ProcessSections command, CancellationToken cancellationToken)
        {
            int created = 0,
                updated = 0,
                deleted = 0;

            logger.LogInformation("Processing sections to {Action}.", command.IsDeleteContext ? "delete" : "create or update");

            for (var i = 0; i < command.BatchSize; i++)
            {
                var nextSection = await dispatcher.DispatchAsync(new GetNextSection(), cancellationToken).WithoutCapturingContext();
                if (nextSection is null ||
                    ((nextSection.IsToBeCreated() || nextSection.IsToBeUpdated()) && command.IsDeleteContext) ||
                    (nextSection.IsToBeDeleted() && !command.IsDeleteContext))
                {
                    logger.LogInformation("No (more) sections to {Action}. Ending batch run.", command.IsDeleteContext ? "delete" : "create or update");
                    break;
                }

                if (nextSection.IsToBeCreated())
                {
                    await dispatcher.DispatchAsync(new CreateSection(nextSection), cancellationToken).WithoutCapturingContext();
                    created++;
                }
                else if (nextSection.IsToBeUpdated())
                {
                    await dispatcher.DispatchAsync(new UpdateSection(nextSection), cancellationToken).WithoutCapturingContext();
                    updated++;
                }
                else if (nextSection.IsToBeDeleted())
                {
                    await dispatcher.DispatchAsync(new DeleteSection(nextSection), cancellationToken).WithoutCapturingContext();
                    deleted++;
                }
            }

            logger.LogInformation("Processed {Processed} section(s) in total.", created + updated + deleted);
            if (command.IsDeleteContext)
            {
                logger.LogDebug("Deleted {Deleted} section(s).", deleted);
            }
            else
            {
                logger.LogDebug("Created {Created} section(s).", created);
                logger.LogDebug("Updated {Updated} section(s).", updated);
            }
        }
    }
}
