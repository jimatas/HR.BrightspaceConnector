using HR.BrightspaceConnector.Features.Common.Commands;
using HR.BrightspaceConnector.Utilities;
using HR.Common.Cqrs.Commands;
using HR.Common.Utilities;

namespace HR.BrightspaceConnector.Features.OrgUnits.Commands
{
    public class DeleteOrgUnit : ICommand
    {
        public DeleteOrgUnit(OrgUnitRecord orgUnit)
        {
            OrgUnit = orgUnit;
        }

        public OrgUnitRecord OrgUnit { get; }
    }

    public class DeleteOrgUnitHandler : ICommandHandler<DeleteOrgUnit>
    {
        private readonly IApiClient apiClient;
        private readonly ICommandDispatcher commandDispatcher;
        private readonly ILogger logger;

        public DeleteOrgUnitHandler(IApiClient apiClient, ICommandDispatcher commandDispatcher, ILogger<DeleteOrgUnit> logger)
        {
            this.apiClient = apiClient;
            this.commandDispatcher = commandDispatcher;
            this.logger = logger;
        }

        public async Task HandleAsync(DeleteOrgUnit command, CancellationToken cancellationToken)
        {
            OrgUnitRecord orgUnit = command.OrgUnit;
            int orgUnitId = Convert.ToInt32(orgUnit.SyncExternalKey);
            int eventId = (int)orgUnit.SyncEventId!;

            logger.LogInformation("Deleting org unit with code \"{OrgUnitCode}\" from Brightspace.", orgUnit.Code);

            try
            {
                await apiClient.DeleteOrgUnitAsync(orgUnitId, permanently: true, cancellationToken).WithoutCapturingContext();
                logger.LogInformation("Org unit was successfully deleted.");

                await commandDispatcher.DispatchAsync(MarkAsHandled.Successfully(eventId, orgUnitId), cancellationToken).WithoutCapturingContext();
            }
            catch (ApiException exception)
            {
                logger.LogWarning(exception, "Error while deleting org unit. {ErrorMessage}", exception.GetErrorMessage());

                await commandDispatcher.DispatchAsync(MarkAsHandled.Unsuccessfully(eventId, orgUnitId, exception.GetErrorMessage()), cancellationToken).WithoutCapturingContext();
            }
        }
    }
}
