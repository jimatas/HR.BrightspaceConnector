using HR.BrightspaceConnector.Features.OrgUnits.Queries;
using HR.Common.Cqrs;
using HR.Common.Cqrs.Queries;
using HR.Common.Utilities;

namespace HR.BrightspaceConnector.Features.OrgUnits.Decorators
{
    /// <summary>
    /// Decorates the query that retrieves the next org unit record from the database. This record may contain an incorrect org unit type ID. 
    /// Using the org unit type code, which is also included in the record, the correct type ID is looked up from Brightspace and amended in the record before it is returned.
    /// </summary>
    public class FixIncorrectOrgUnitType : IQueryHandlerWrapper<GetNextOrgUnit, OrgUnitRecord?>
    {
        private readonly IApiClient apiClient;

        public FixIncorrectOrgUnitType(IApiClient apiClient)
        {
            this.apiClient = apiClient;
        }

        public async Task<OrgUnitRecord?> HandleAsync(GetNextOrgUnit query, HandlerDelegate<OrgUnitRecord?> next, CancellationToken cancellationToken)
        {
            OrgUnitRecord? retrievedOrgUnit = await next().WithoutCapturingContext();
            if (retrievedOrgUnit is not null)
            {
                var orgUnitTypes = await apiClient.GetOrgUnitTypesAsync(cancellationToken).WithoutCapturingContext();
                var orgUnitType = orgUnitTypes.SingleOrDefault(type => string.Equals(type.Code, retrievedOrgUnit.TypeCode, StringComparison.OrdinalIgnoreCase));
                if (orgUnitType is not null)
                {
                    if (retrievedOrgUnit.Type != orgUnitType.Id)
                    {
                        retrievedOrgUnit.Type = orgUnitType.Id;
                    }

                    if (!string.Equals(retrievedOrgUnit.TypeCode, orgUnitType.Code, StringComparison.Ordinal))
                    {
                        retrievedOrgUnit.TypeCode = orgUnitType.Code;
                    }
                }
            }
            return retrievedOrgUnit;
        }
    }
}
