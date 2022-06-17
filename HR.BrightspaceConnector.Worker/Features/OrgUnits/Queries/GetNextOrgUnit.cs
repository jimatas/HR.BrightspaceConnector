using HR.BrightspaceConnector.Infrastructure.Persistence;
using HR.Common.Cqrs.Queries;
using HR.Common.Utilities;

namespace HR.BrightspaceConnector.Features.OrgUnits.Queries
{
    public class GetNextOrgUnit : IQuery<OrgUnitRecord?>
    {
    }

    public class GetNextOrgUnitHandler : IQueryHandler<GetNextOrgUnit, OrgUnitRecord?>
    {
        private readonly IDatabase database;
        private readonly ILogger logger;

        public GetNextOrgUnitHandler(IDatabase database, ILogger<GetNextOrgUnit> logger)
        {
            this.database = database;
            this.logger = logger;
        }

        public async Task<OrgUnitRecord?> HandleAsync(GetNextOrgUnit query, CancellationToken cancellationToken)
        {
            var orgUnit = await database.GetNextOrgUnitAsync(cancellationToken).WithoutCapturingContext();
            if (orgUnit is not null)
            {
                logger.LogInformation("Retrieved org unit with name \"{Name}\" for sync action '{SyncAction}' from database.", orgUnit.Name, orgUnit.SyncAction);
            }

            return orgUnit;
        }
    }
}
