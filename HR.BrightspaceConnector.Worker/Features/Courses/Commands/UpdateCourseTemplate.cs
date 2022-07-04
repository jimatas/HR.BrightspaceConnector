using HR.BrightspaceConnector.Features.Common.Commands;
using HR.BrightspaceConnector.Utilities;
using HR.Common.Cqrs.Commands;
using HR.Common.Utilities;

namespace HR.BrightspaceConnector.Features.Courses.Commands
{
    public class UpdateCourseTemplate : ICommand
    {
        public UpdateCourseTemplate(CourseTemplateRecord courseTemplate)
        {
            CourseTemplate = courseTemplate;
        }

        public CourseTemplateRecord CourseTemplate { get; }
    }

    public class UpdateCourseTemplateHandler : ICommandHandler<UpdateCourseTemplate>
    {
        private readonly IApiClient apiClient;
        private readonly ICommandDispatcher commandDispatcher;
        private readonly ILogger logger;

        public UpdateCourseTemplateHandler(IApiClient apiClient, ICommandDispatcher commandDispatcher, ILogger<UpdateCourseTemplate> logger)
        {
            this.apiClient = apiClient;
            this.commandDispatcher = commandDispatcher;
            this.logger = logger;
        }

        public async Task HandleAsync(UpdateCourseTemplate command, CancellationToken cancellationToken)
        {
            CourseTemplateRecord courseTemplate = command.CourseTemplate;
            int courseTemplateId = Convert.ToInt32(courseTemplate.SyncExternalKey);
            int eventId = (int)courseTemplate.SyncEventId!;

            logger.LogInformation("Updating course template with code \"{CourseTemplateCode}\" in Brightspace.", courseTemplate.Code);

            try
            {
                await apiClient.UpdateCourseTemplateAsync(courseTemplateId, courseTemplate.ToCourseTemplateInfo(), cancellationToken).WithoutCapturingContext();
                logger.LogInformation("Course template was successfully updated.");

                await commandDispatcher.DispatchAsync(MarkAsHandled.Successfully(eventId, courseTemplateId), cancellationToken).WithoutCapturingContext();
            }
            catch (ApiException exception)
            {
                logger.LogWarning(exception, "Error while updating course template. {ErrorMessage}", exception.GetErrorMessage());

                await commandDispatcher.DispatchAsync(MarkAsHandled.Unsuccessfully(eventId, courseTemplateId, exception.GetErrorMessage()), cancellationToken).WithoutCapturingContext();
            }
        }
    }
}
