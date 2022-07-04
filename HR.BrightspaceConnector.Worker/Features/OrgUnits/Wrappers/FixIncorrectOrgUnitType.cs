using HR.BrightspaceConnector.Features.OrgUnits.Commands;
using HR.Common.Cqrs;
using HR.Common.Cqrs.Commands;
using HR.Common.Utilities;

namespace HR.BrightspaceConnector.Features.OrgUnits.Wrappers
{
    public class FixIncorrectOrgUnitType :
        ICommandHandlerWrapper<CreateOrgUnit>,
        ICommandHandlerWrapper<UpdateOrgUnit>
    {
        private readonly IApiClient apiClient;

        public FixIncorrectOrgUnitType(IApiClient apiClient)
        {
            this.apiClient = apiClient;
        }

        public async Task HandleAsync(CreateOrgUnit command, HandlerDelegate next, CancellationToken cancellationToken)
        {
            await HandleInternalAsync(command.OrgUnit, next, cancellationToken).WithoutCapturingContext();
        }

        public async Task HandleAsync(UpdateOrgUnit command, HandlerDelegate next, CancellationToken cancellationToken)
        {
            await HandleInternalAsync(command.OrgUnit, next, cancellationToken).WithoutCapturingContext();
        }

        private async Task HandleInternalAsync(OrgUnitRecord orgUnit, HandlerDelegate next, CancellationToken cancellationToken)
        {
            var orgUnitTypes = await apiClient.GetOrgUnitTypesAsync(cancellationToken).WithoutCapturingContext();
            var orgUnitType = orgUnitTypes.SingleOrDefault(type => string.Equals(type.Code, orgUnit.TypeCode, StringComparison.OrdinalIgnoreCase));
            if (orgUnitType is not null)
            {
                if (orgUnit.Type != orgUnitType.Id)
                {
                    orgUnit.Type = orgUnitType.Id;
                }

                if (!string.Equals(orgUnit.TypeCode, orgUnitType.Code, StringComparison.Ordinal))
                {
                    orgUnit.TypeCode = orgUnitType.Code;
                }
            }

            await next().WithoutCapturingContext();
        }
    }
}
