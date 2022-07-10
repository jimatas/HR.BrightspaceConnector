using HR.BrightspaceConnector.Features.Common.Commands;
using HR.BrightspaceConnector.Utilities;
using HR.Common.Cqrs.Commands;
using HR.Common.Utilities;

namespace HR.BrightspaceConnector.Features.Courses.Commands
{
    public class UpdateCourseOffering : ICommand
    {
        public UpdateCourseOffering(CourseOfferingRecord courseOffering)
        {
            CourseOffering = courseOffering;
        }

        public CourseOfferingRecord CourseOffering { get; }
    }

    public class UpdateCourseOfferingHandler : ICommandHandler<UpdateCourseOffering>
    {
        private readonly IApiClient apiClient;
        private readonly ICommandDispatcher commandDispatcher;
        private readonly ILogger logger;

        public UpdateCourseOfferingHandler(IApiClient apiClient, ICommandDispatcher commandDispatcher, ILogger<UpdateCourseOffering> logger)
        {
            this.apiClient = apiClient;
            this.commandDispatcher = commandDispatcher;
            this.logger = logger;
        }

        public async Task HandleAsync(UpdateCourseOffering command, CancellationToken cancellationToken)
        {
            CourseOfferingRecord courseOffering = command.CourseOffering;
            int courseOfferingId = Convert.ToInt32(courseOffering.SyncExternalKey);
            int eventId = (int)courseOffering.SyncEventId!;

            logger.LogInformation("Updating course offering with code \"{CourseOfferingCode}\" in Brightspace.", courseOffering.Code);

            try
            {
                await apiClient.UpdateCourseOfferingAsync(courseOfferingId, courseOffering.ToCourseOfferingInfo(), cancellationToken).WithoutCapturingContext();
                logger.LogInformation("Course offering was successfully updated.");

                await commandDispatcher.DispatchAsync(MarkAsHandled.Successfully(eventId, courseOfferingId), cancellationToken).WithoutCapturingContext();
            }
            catch (ApiException exception)
            {
                logger.LogWarning(exception, "Error while updating course offering. {ErrorMessage}", exception.GetErrorMessage());

                await commandDispatcher.DispatchAsync(MarkAsHandled.Unsuccessfully(eventId, courseOfferingId, exception.GetErrorMessage()), cancellationToken).WithoutCapturingContext();
            }
        }
    }
}
