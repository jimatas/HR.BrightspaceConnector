using HR.BrightspaceConnector.Infrastructure.Persistence;
using HR.Common.Cqrs.Queries;
using HR.Common.Utilities;

namespace HR.BrightspaceConnector.Features.Enrollments.Queries
{
    public class GetNextEnrollment : IQuery<EnrollmentRecord?>
    {
    }

    public class GetNextEnrollmentHandler : IQueryHandler<GetNextEnrollment, EnrollmentRecord?>
    {
        private readonly IDatabase database;
        private readonly ILogger logger;

        public GetNextEnrollmentHandler(IDatabase database, ILogger<GetNextEnrollment> logger)
        {
            this.database = database;
            this.logger = logger;
        }

        public async Task<EnrollmentRecord?> HandleAsync(GetNextEnrollment query, CancellationToken cancellationToken)
        {
            EnrollmentRecord? enrollment = await database.GetNextCourseOfferingEnrollmentAsync(cancellationToken).WithoutCapturingContext();
            if (enrollment is not null)
            {
                logger.LogInformation("Retrieved enrollment of user with ID {UserId} in org unit with ID {OrgUnitId} for sync action '{SyncAction}' from database.",
                    enrollment.UserId, enrollment.OrgUnitId, enrollment.SyncAction);
            }

            return enrollment;
        }
    }
}
