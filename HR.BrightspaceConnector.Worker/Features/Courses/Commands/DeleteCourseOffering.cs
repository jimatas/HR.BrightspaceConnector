using HR.BrightspaceConnector.Features.Common.Commands;
using HR.BrightspaceConnector.Utilities;
using HR.Common.Cqrs.Commands;
using HR.Common.Utilities;

namespace HR.BrightspaceConnector.Features.Courses.Commands
{
    public class DeleteCourseOffering : ICommand
    {
        public DeleteCourseOffering(CourseOfferingRecord courseOffering)
        {
            CourseOffering = courseOffering;
        }

        public CourseOfferingRecord CourseOffering { get; }
    }

    public class DeleteCourseOfferingHandler : ICommandHandler<DeleteCourseOffering>
    {
        private readonly IApiClient apiClient;
        private readonly ICommandDispatcher commandDispatcher;
        private readonly ILogger logger;

        public DeleteCourseOfferingHandler(IApiClient apiClient, ICommandDispatcher commandDispatcher, ILogger<DeleteCourseOffering> logger)
        {
            this.apiClient = apiClient;
            this.commandDispatcher = commandDispatcher;
            this.logger = logger;
        }

        public async Task HandleAsync(DeleteCourseOffering command, CancellationToken cancellationToken)
        {
            CourseOfferingRecord courseOffering = command.CourseOffering;
            int courseOfferingId = Convert.ToInt32(courseOffering.SyncExternalKey);
            int eventId = (int)courseOffering.SyncEventId!;

            logger.LogInformation("Deleting course offering with code \"{CourseOfferingCode}\" from Brightspace.", courseOffering.Code);

            try
            {
                await apiClient.DeleteCourseOfferingAsync(courseOfferingId, permanently: true, cancellationToken).WithoutCapturingContext();
                logger.LogInformation("Course offering was successfully deleted.");

                await commandDispatcher.DispatchAsync(MarkAsHandled.Successfully(eventId, courseOfferingId), cancellationToken).WithoutCapturingContext();
            }
            catch (ApiException exception)
            {
                logger.LogWarning(exception, "Error while deleting course offering. {ErrorMessage}", exception.GetErrorMessage());

                await commandDispatcher.DispatchAsync(MarkAsHandled.Unsuccessfully(eventId, courseOfferingId, exception.GetErrorMessage()), cancellationToken).WithoutCapturingContext();
            }
        }
    }
}
