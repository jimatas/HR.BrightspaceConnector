using HR.BrightspaceConnector.Features.Common.Commands;
using HR.BrightspaceConnector.Utilities;
using HR.Common.Cqrs.Commands;
using HR.Common.Utilities;

namespace HR.BrightspaceConnector.Features.OrgUnits.Commands
{
    public class UpdateOrgUnit : ICommand
    {
        public UpdateOrgUnit(OrgUnitRecord orgUnit)
        {
            OrgUnit = orgUnit;
        }

        public OrgUnitRecord OrgUnit { get; }
    }

    public class UpdateOrgUnitHandler : ICommandHandler<UpdateOrgUnit>
    {
        private readonly IApiClient apiClient;
        private readonly ICommandDispatcher commandDispatcher;
        private readonly ILogger logger;

        public UpdateOrgUnitHandler(IApiClient apiClient, ICommandDispatcher commandDispatcher, ILogger<UpdateOrgUnit> logger)
        {
            this.apiClient = apiClient;
            this.commandDispatcher = commandDispatcher;
            this.logger = logger;
        }

        public async Task HandleAsync(UpdateOrgUnit command, CancellationToken cancellationToken)
        {
            OrgUnitRecord orgUnit = command.OrgUnit;
            int orgUnitId = Convert.ToInt32(orgUnit.SyncExternalKey);
            int eventId = (int)orgUnit.SyncEventId!;

            logger.LogInformation("Updating org unit with code \"{Code}\" in Brightspace.", orgUnit.Code);

            try
            {
                var updatedOrgUnit = await apiClient.UpdateOrgUnitAsync(orgUnitId, orgUnit.ToOrgUnitProperties(), cancellationToken).WithoutCapturingContext();
                logger.LogInformation("Org unit was successfully updated.");

                await commandDispatcher.DispatchAsync(MarkAsHandled.Successfully(eventId, orgUnitId), cancellationToken).WithoutCapturingContext();
            }
            catch (ApiException exception)
            {
                logger.LogWarning(exception, "Error while updating org unit. {ErrorMessage}", exception.GetErrorMessage());

                await commandDispatcher.DispatchAsync(MarkAsHandled.Unsuccessfully(eventId, orgUnitId, exception.GetErrorMessage()), cancellationToken).WithoutCapturingContext();
            }
        }
    }
}
