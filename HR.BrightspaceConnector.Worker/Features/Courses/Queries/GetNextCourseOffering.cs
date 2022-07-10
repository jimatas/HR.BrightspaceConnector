using HR.BrightspaceConnector.Infrastructure.Persistence;
using HR.Common.Cqrs.Queries;
using HR.Common.Utilities;

namespace HR.BrightspaceConnector.Features.Courses.Queries
{
    public class GetNextCourseOffering : IQuery<CourseOfferingRecord?>
    {
    }

    public class GetNextCourseOfferingHandler : IQueryHandler<GetNextCourseOffering, CourseOfferingRecord?>
    {
        private readonly IDatabase database;
        private readonly ILogger logger;

        public GetNextCourseOfferingHandler(IDatabase database, ILogger<GetNextCourseOffering> logger)
        {
            this.database = database;
            this.logger = logger;
        }

        public async Task<CourseOfferingRecord?> HandleAsync(GetNextCourseOffering query, CancellationToken cancellationToken)
        {
            CourseOfferingRecord? courseOffering = await database.GetNextCourseOfferingAsync(cancellationToken).WithoutCapturingContext();
            if (courseOffering is not null)
            {
                logger.LogInformation("Retrieved course offering with code \"{CourseOfferingCode}\" for sync action '{SyncAction}' from database.", courseOffering.Code, courseOffering.SyncAction);
            }

            return courseOffering;
        }
    }
}
