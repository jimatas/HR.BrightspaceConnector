using HR.BrightspaceConnector.Features.Common.Commands;
using HR.BrightspaceConnector.Utilities;
using HR.Common.Cqrs.Commands;
using HR.Common.Utilities;

namespace HR.BrightspaceConnector.Features.OrgUnits.Commands
{
    public class CreateOrgUnit : ICommand
    {
        public CreateOrgUnit(OrgUnitRecord orgUnit)
        {
            OrgUnit = orgUnit;
        }

        public OrgUnitRecord OrgUnit { get; }
    }

    public class CreateOrgUnitHandler : ICommandHandler<CreateOrgUnit>
    {
        private readonly IApiClient apiClient;
        private readonly ICommandDispatcher commandDispatcher;
        private readonly ILogger logger;

        public CreateOrgUnitHandler(IApiClient apiClient, ICommandDispatcher commandDispatcher, ILogger<CreateOrgUnit> logger)
        {
            this.apiClient = apiClient;
            this.commandDispatcher = commandDispatcher;
            this.logger = logger;
        }

        public async Task HandleAsync(CreateOrgUnit command, CancellationToken cancellationToken)
        {
            OrgUnitRecord orgUnit = command.OrgUnit;
            int eventId = (int)orgUnit.SyncEventId!;

            logger.LogInformation("Creating org unit with code \"{OrgUnitCode}\" in Brightspace.", orgUnit.Code);

            try
            {
                var orgUnitCreateData = await orgUnit.ToOrgUnitCreateDataAsync(apiClient, cancellationToken).WithoutCapturingContext();
                var newOrgUnit = await apiClient.CreateOrgUnitAsync(orgUnitCreateData, cancellationToken).WithoutCapturingContext();
                logger.LogInformation("Org unit was successfully created.");

                await commandDispatcher.DispatchAsync(MarkAsHandled.Successfully(eventId, (int)newOrgUnit.Identifier!), cancellationToken).WithoutCapturingContext();
            }
            catch (ApiException exception)
            {
                logger.LogWarning(exception, "Error while creating org unit. {ErrorMessage}", exception.GetErrorMessage());

                await commandDispatcher.DispatchAsync(MarkAsHandled.Unsuccessfully(eventId, exception.GetErrorMessage()), cancellationToken).WithoutCapturingContext();
            }
        }
    }
}
