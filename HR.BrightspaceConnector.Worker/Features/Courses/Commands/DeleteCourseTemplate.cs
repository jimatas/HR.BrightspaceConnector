using HR.BrightspaceConnector.Features.Common.Commands;
using HR.BrightspaceConnector.Utilities;
using HR.Common.Cqrs.Commands;
using HR.Common.Utilities;

namespace HR.BrightspaceConnector.Features.Courses.Commands
{
    public class DeleteCourseTemplate : ICommand
    {
        public DeleteCourseTemplate(CourseTemplateRecord courseTemplate)
        {
            CourseTemplate = courseTemplate;
        }

        public CourseTemplateRecord CourseTemplate { get; }
    }

    public class DeleteCourseTemplateHandler : ICommandHandler<DeleteCourseTemplate>
    {
        private readonly IApiClient apiClient;
        private readonly ICommandDispatcher commandDispatcher;
        private readonly ILogger logger;

        public DeleteCourseTemplateHandler(IApiClient apiClient, ICommandDispatcher commandDispatcher, ILogger<DeleteCourseTemplate> logger)
        {
            this.apiClient = apiClient;
            this.commandDispatcher = commandDispatcher;
            this.logger = logger;
        }

        public async Task HandleAsync(DeleteCourseTemplate command, CancellationToken cancellationToken)
        {
            CourseTemplateRecord courseTemplate = command.CourseTemplate;
            int courseTemplateId = Convert.ToInt32(courseTemplate.SyncExternalKey);
            int eventId = (int)courseTemplate.SyncEventId!;

            logger.LogInformation("Deleting course template with code \"{CourseTemplateCode}\" from Brightspace.", courseTemplate.Code);

            try
            {
                await apiClient.DeleteCourseTemplateAsync(courseTemplateId, permanently: true, cancellationToken).WithoutCapturingContext();
                logger.LogInformation("Course template was successfully deleted.");

                await commandDispatcher.DispatchAsync(MarkAsHandled.Successfully(eventId, courseTemplateId), cancellationToken).WithoutCapturingContext();
            }
            catch (ApiException exception)
            {
                logger.LogWarning(exception, "Error while deleting course template. {ErrorMessage}", exception.GetErrorMessage());

                await commandDispatcher.DispatchAsync(MarkAsHandled.Unsuccessfully(eventId, courseTemplateId, exception.GetErrorMessage()), cancellationToken).WithoutCapturingContext();
            }
        }
    }
}
