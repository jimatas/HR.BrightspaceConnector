using HR.BrightspaceConnector.Features.OrgUnits.Commands;
using HR.Common.Cqrs;
using HR.Common.Cqrs.Commands;
using HR.Common.Utilities;

namespace HR.BrightspaceConnector.Features.OrgUnits.Wrappers
{
    public class EnsureCorrectOrgUnitType : 
        ICommandHandlerWrapper<CreateOrgUnit>, 
        ICommandHandlerWrapper<UpdateOrgUnit>
    {
        private readonly IApiClient apiClient;

        public EnsureCorrectOrgUnitType(IApiClient apiClient)
        {
            this.apiClient = apiClient;
        }

        public async Task HandleAsync(CreateOrgUnit command, HandlerDelegate next, CancellationToken cancellationToken)
        {
            OrgUnitRecord orgUnitToCreate = command.OrgUnit;
            var orgUnitTypes = await apiClient.GetOrgUnitTypesAsync(cancellationToken).WithoutCapturingContext();
            var orgUnitType = orgUnitTypes.SingleOrDefault(type => string.Equals(type.Code, orgUnitToCreate.TypeCode, StringComparison.OrdinalIgnoreCase));
            if (orgUnitType is not null && orgUnitToCreate.Type != orgUnitType.Id)
            {
                orgUnitToCreate.Type = orgUnitType.Id;
            }

            await next().WithoutCapturingContext();
        }

        public async Task HandleAsync(UpdateOrgUnit command, HandlerDelegate next, CancellationToken cancellationToken)
        {
            OrgUnitRecord orgUnitToUpdate = command.OrgUnit;
            var orgUnitTypes = await apiClient.GetOrgUnitTypesAsync(cancellationToken).WithoutCapturingContext();
            var orgUnitType = orgUnitTypes.SingleOrDefault(type => string.Equals(type.Code, orgUnitToUpdate.TypeCode, StringComparison.OrdinalIgnoreCase));
            if (orgUnitType is not null && (orgUnitToUpdate.Type != orgUnitType.Id || !string.Equals(orgUnitToUpdate.TypeCode, orgUnitType.Code, StringComparison.Ordinal)))
            {
                orgUnitToUpdate.Type = orgUnitType.Id;
                orgUnitToUpdate.TypeCode = orgUnitType.Code;
            }

            await next().WithoutCapturingContext();
        }
    }
}
