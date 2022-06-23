using HR.BrightspaceConnector.Infrastructure.Persistence;
using HR.Common.Cqrs.Queries;
using HR.Common.Utilities;

namespace HR.BrightspaceConnector.Features.OrgUnits.Queries
{
    public class GetNextOrgUnit : IQuery<OrgUnitRecord?>
    {
        public GetNextOrgUnit(bool isDepartmentType = false)
        {
            IsDepartmentType = isDepartmentType;
        }

        public bool IsDepartmentType { get; }
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
            OrgUnitRecord? orgUnit = query.IsDepartmentType
                ? await database.GetNextDepartmentAsync(cancellationToken).WithoutCapturingContext()
                : await database.GetNextCustomOrgUnitAsync(cancellationToken).WithoutCapturingContext();

            if (orgUnit is not null)
            {
                logger.LogInformation("Retrieved org unit of type \"{OrgUnitType}\" with code \"{OrgUnitCode}\" for sync action '{SyncAction}' from database.",
                    query.IsDepartmentType ? "department" : "customOrgUnit", orgUnit.Code, orgUnit.SyncAction);
            }

            return orgUnit;
        }
    }
}
