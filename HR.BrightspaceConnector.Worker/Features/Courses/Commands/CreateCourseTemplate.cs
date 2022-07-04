using HR.BrightspaceConnector.Features.Common.Commands;
using HR.BrightspaceConnector.Utilities;
using HR.Common.Cqrs.Commands;
using HR.Common.Utilities;

namespace HR.BrightspaceConnector.Features.Courses.Commands
{
    public class CreateCourseTemplate : ICommand
    {
        public CreateCourseTemplate(CourseTemplateRecord courseTemplate)
        {
            CourseTemplate = courseTemplate;
        }

        public CourseTemplateRecord CourseTemplate { get; }
    }

    public class CreateCourseTemplateHandler : ICommandHandler<CreateCourseTemplate>
    {
        private readonly IApiClient apiClient;
        private readonly ICommandDispatcher commandDispatcher;
        private readonly ILogger logger;

        public CreateCourseTemplateHandler(IApiClient apiClient, ICommandDispatcher commandDispatcher, ILogger<CreateCourseTemplate> logger)
        {
            this.apiClient = apiClient;
            this.commandDispatcher = commandDispatcher;
            this.logger = logger;
        }

        public async Task HandleAsync(CreateCourseTemplate command, CancellationToken cancellationToken)
        {
            CourseTemplateRecord courseTemplate = command.CourseTemplate;
            int eventId = (int)courseTemplate.SyncEventId!;

            logger.LogInformation("Creating course template with code \"{CourseTemplateCode}\" in Brightspace.", courseTemplate.Code);

            try
            {
                var newCourseTemplate = await apiClient.CreateCourseTemplateAsync(courseTemplate.ToCreateCourseTemplate(), cancellationToken).WithoutCapturingContext();
                logger.LogInformation("Course template was successfully created.");

                await commandDispatcher.DispatchAsync(MarkAsHandled.Successfully(eventId, (int)newCourseTemplate.Identifier!), cancellationToken).WithoutCapturingContext();
            }
            catch (ApiException exception)
            {
                logger.LogWarning(exception, "Error while creating course template. {ErrorMessage}", exception.GetErrorMessage());

                await commandDispatcher.DispatchAsync(MarkAsHandled.Unsuccessfully(eventId, exception.GetErrorMessage()), cancellationToken).WithoutCapturingContext();
            }
        }
    }
}
