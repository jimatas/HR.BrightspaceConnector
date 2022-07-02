using HR.BrightspaceConnector.Features.OrgUnits.Commands;
using HR.BrightspaceConnector.Infrastructure;
using HR.Common.Cqrs;
using HR.Common.Cqrs.Commands;
using HR.Common.Utilities;

namespace HR.BrightspaceConnector.Features.OrgUnits.Wrappers
{
    public class EnsureOrgUnitPath : ICommandHandlerWrapper<UpdateOrgUnit>
    {
        private readonly IApiClient apiClient;

        public EnsureOrgUnitPath(IApiClient apiClient)
        {
            this.apiClient = apiClient;
        }

        public async Task HandleAsync(UpdateOrgUnit command, HandlerDelegate next, CancellationToken cancellationToken)
        {
            OrgUnitRecord orgUnit = command.OrgUnit;
            if (string.IsNullOrEmpty(orgUnit.Path))
            {
                int orgUnitId = Convert.ToInt32(orgUnit.SyncExternalKey);
                var queryParams = new OrgUnitQueryParameters { ExactOrgUnitCode = orgUnit.Code };
                PagedResultSet<OrgUnitProperties> existingOrgUnits;
                do
                {
                    existingOrgUnits = await apiClient.GetOrgUnitsAsync(queryParams, cancellationToken).WithoutCapturingContext();
                    var existingOrgUnit = existingOrgUnits.FirstOrDefault(ou => ou.Identifier == orgUnitId);
                    if (existingOrgUnit is not null)
                    {
                        orgUnit.Path = existingOrgUnit.Path;
                        break;
                    }
                    queryParams.Bookmark = existingOrgUnits.PagingInfo.Bookmark;
                }
                while (existingOrgUnits.PagingInfo.HasMoreItems);
            }

            await next().WithoutCapturingContext();
        }
    }
}
