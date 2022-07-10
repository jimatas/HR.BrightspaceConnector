using HR.BrightspaceConnector.Features.Common.Commands;
using HR.BrightspaceConnector.Utilities;
using HR.Common.Cqrs.Commands;
using HR.Common.Utilities;

namespace HR.BrightspaceConnector.Features.Courses.Commands
{
    public class CreateCourseOffering : ICommand
    {
        public CreateCourseOffering(CourseOfferingRecord courseOffering)
        {
            CourseOffering = courseOffering;
        }

        public CourseOfferingRecord CourseOffering { get; }
    }

    public class CreateCourseOfferingHandler : ICommandHandler<CreateCourseOffering>
    {
        private readonly IApiClient apiClient;
        private readonly ICommandDispatcher commandDispatcher;
        private readonly ILogger logger;

        public CreateCourseOfferingHandler(IApiClient apiClient, ICommandDispatcher commandDispatcher, ILogger<CreateCourseOffering> logger)
        {
            this.apiClient = apiClient;
            this.commandDispatcher = commandDispatcher;
            this.logger = logger;
        }

        public async Task HandleAsync(CreateCourseOffering command, CancellationToken cancellationToken)
        {
            CourseOfferingRecord courseOffering = command.CourseOffering;
            int eventId = (int)courseOffering.SyncEventId!;

            logger.LogInformation("Creating course offering with code \"{CourseOfferingCode}\" in Brightspace.", courseOffering.Code);

            try
            {
                var newCourseOffering = await apiClient.CreateCourseOfferingAsync(courseOffering.ToCreateCourseOffering(), cancellationToken).WithoutCapturingContext();
                logger.LogInformation("Course offering was successfully created.");

                await commandDispatcher.DispatchAsync(MarkAsHandled.Successfully(eventId, (int)newCourseOffering.Identifier!), cancellationToken).WithoutCapturingContext();
            }
            catch (ApiException exception)
            {
                logger.LogWarning(exception, "Error while creating course offering. {ErrorMessage}", exception.GetErrorMessage());

                await commandDispatcher.DispatchAsync(MarkAsHandled.Unsuccessfully(eventId, exception.GetErrorMessage()), cancellationToken).WithoutCapturingContext();
            }
        }
    }
}
