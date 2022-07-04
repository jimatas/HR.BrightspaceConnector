using HR.BrightspaceConnector.Infrastructure.Persistence;
using HR.Common.Cqrs.Queries;
using HR.Common.Utilities;

namespace HR.BrightspaceConnector.Features.Courses.Queries
{
    public class GetNextCourseTemplate : IQuery<CourseTemplateRecord?>
    {
    }

    public class GetNextCourseTemplateHandler : IQueryHandler<GetNextCourseTemplate, CourseTemplateRecord?>
    {
        private readonly IDatabase database;
        private readonly ILogger logger;

        public GetNextCourseTemplateHandler(IDatabase database, ILogger<GetNextCourseTemplate> logger)
        {
            this.database = database;
            this.logger = logger;
        }

        public async Task<CourseTemplateRecord?> HandleAsync(GetNextCourseTemplate query, CancellationToken cancellationToken)
        {
            CourseTemplateRecord? courseTemplate = await database.GetNextCourseTemplateAsync(cancellationToken).WithoutCapturingContext();
            if (courseTemplate is not null)
            {
                logger.LogInformation("Retrieved course template with code \"{CourseTemplateCode}\" for sync action '{SyncAction}' from database.", courseTemplate.Code, courseTemplate.SyncAction);
            }

            return courseTemplate;
        }
    }
}
