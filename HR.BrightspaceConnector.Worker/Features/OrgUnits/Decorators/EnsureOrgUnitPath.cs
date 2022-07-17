using HR.BrightspaceConnector.Features.OrgUnits.Commands;
using HR.BrightspaceConnector.Infrastructure;
using HR.Common.Cqrs;
using HR.Common.Cqrs.Commands;
using HR.Common.Utilities;

namespace HR.BrightspaceConnector.Features.OrgUnits.Decorators
{
    /// <summary>
    /// Decorates the command to update an org unit in Brightspace. The org unit retrieved from the database may not have its path set, while the update route for org units expects it to be passed in.
    /// This decorator will look up the path from Brightspace and update the command with it if necessary.
    /// </summary>
    public class EnsureOrgUnitPath : ICommandHandlerWrapper<UpdateOrgUnit>
    {
        private readonly IApiClient apiClient;

        public EnsureOrgUnitPath(IApiClient apiClient)
        {
            this.apiClient = apiClient;
        }

        public async Task HandleAsync(UpdateOrgUnit command, HandlerDelegate next, CancellationToken cancellationToken)
        {
            OrgUnitRecord orgUnitToUpdate = command.OrgUnit;
            if (string.IsNullOrEmpty(orgUnitToUpdate.Path))
            {
                int orgUnitId = Convert.ToInt32(orgUnitToUpdate.SyncExternalKey);
                var queryParams = new OrgUnitQueryParameters { ExactOrgUnitCode = orgUnitToUpdate.Code };
                PagedResultSet<OrgUnitProperties> existingOrgUnits;
                do
                {
                    existingOrgUnits = await apiClient.GetOrgUnitsAsync(queryParams, cancellationToken).WithoutCapturingContext();
                    var existingOrgUnit = existingOrgUnits.FirstOrDefault(ou => ou.Identifier == orgUnitId);
                    if (existingOrgUnit is not null)
                    {
                        orgUnitToUpdate.Path = existingOrgUnit.Path;
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
