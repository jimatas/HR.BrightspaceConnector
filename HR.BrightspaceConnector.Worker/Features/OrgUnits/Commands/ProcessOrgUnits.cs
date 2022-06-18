using HR.BrightspaceConnector.Features.Common;
using HR.BrightspaceConnector.Features.OrgUnits.Queries;
using HR.Common.Cqrs;
using HR.Common.Cqrs.Commands;
using HR.Common.Utilities;

namespace HR.BrightspaceConnector.Features.OrgUnits.Commands
{
    public class ProcessOrgUnits : ICommand
    {
        public ProcessOrgUnits(int batchSize, bool isDeleteContext = false)
        {
            BatchSize = batchSize;
            IsDeleteContext = isDeleteContext;
        }

        public int BatchSize { get; }
        public bool IsDeleteContext { get; }
    }

    public class ProcessOrgUnitsHandler : ICommandHandler<ProcessOrgUnits>
    {
        private readonly IDispatcher dispatcher;
        private readonly ILogger logger;

        public ProcessOrgUnitsHandler(IDispatcher dispatcher, ILogger<ProcessOrgUnits> logger)
        {
            this.dispatcher = dispatcher;
            this.logger = logger;
        }

        public async Task HandleAsync(ProcessOrgUnits command, CancellationToken cancellationToken)
        {
            int created = 0,
                updated = 0,
                deleted = 0;

            logger.LogInformation("Processing org units to {Action}.", command.IsDeleteContext ? "delete" : "create or update");

            for (var i = 0; i < command.BatchSize; i++)
            {
                var nextOrgUnit = await dispatcher.DispatchAsync(new GetNextOrgUnit(), cancellationToken).WithoutCapturingContext();
                if (nextOrgUnit is null ||
                    ((nextOrgUnit.IsToBeCreated() || nextOrgUnit.IsToBeUpdated()) && command.IsDeleteContext) ||
                    (nextOrgUnit.IsToBeDeleted() && !command.IsDeleteContext))
                {
                    logger.LogInformation("No more org units to {Action}. Ending batch run.", command.IsDeleteContext ? "delete" : "create or update");
                    break;
                }

                if (nextOrgUnit.IsToBeCreated())
                {
                    await dispatcher.DispatchAsync(new CreateOrgUnit(nextOrgUnit), cancellationToken).WithoutCapturingContext();
                    created++;
                }
                else if (nextOrgUnit.IsToBeUpdated())
                {
                    await dispatcher.DispatchAsync(new UpdateOrgUnit(nextOrgUnit), cancellationToken).WithoutCapturingContext();
                    updated++;
                }
                else if (nextOrgUnit.IsToBeDeleted())
                {
                    await dispatcher.DispatchAsync(new DeleteOrgUnit(nextOrgUnit), cancellationToken).WithoutCapturingContext();
                    deleted++;
                }
            }

            logger.LogInformation("Processed {Processed} org unit(s) in total.", created + updated + deleted);
            if (command.IsDeleteContext)
            {
                logger.LogDebug("Deleted {Deleted} org unit(s).", deleted);
            }
            else
            {
                logger.LogDebug("Created {Created} org unit(s).", created);
                logger.LogDebug("Updated {Updated} org unit(s).", updated);
            }
        }
    }
}
