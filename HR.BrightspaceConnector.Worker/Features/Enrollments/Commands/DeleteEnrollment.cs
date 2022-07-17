using HR.BrightspaceConnector.Features.Common.Commands;
using HR.BrightspaceConnector.Utilities;
using HR.Common.Cqrs.Commands;
using HR.Common.Utilities;

namespace HR.BrightspaceConnector.Features.Enrollments.Commands
{
    public class DeleteEnrollment : ICommand
    {
        public DeleteEnrollment(EnrollmentRecord enrollment)
        {
            Enrollment = enrollment;
        }

        public EnrollmentRecord Enrollment { get; set; }
    }

    public class DeleteEnrollmentHandler : ICommandHandler<DeleteEnrollment>
    {
        private readonly IApiClient apiClient;
        private readonly ICommandDispatcher commandDispatcher;
        private readonly ILogger logger;

        public DeleteEnrollmentHandler(IApiClient apiClient, ICommandDispatcher commandDispatcher, ILogger<DeleteEnrollment> logger)
        {
            this.apiClient = apiClient;
            this.commandDispatcher = commandDispatcher;
            this.logger = logger;
        }

        public async Task HandleAsync(DeleteEnrollment command, CancellationToken cancellationToken)
        {
            EnrollmentRecord enrollment = command.Enrollment;
            int eventId = (int)enrollment.SyncEventId!;
            _ = long.TryParse(enrollment.SyncExternalKey, out var enrollmentId);

            logger.LogInformation("Deleting enrollment of user with ID {UserId} in org unit with ID {OrgUnitId} from Brightspace.", enrollment.UserId, enrollment.OrgUnitId);

            try
            {
                await apiClient.DeleteEnrollmentAsync((int)enrollment.UserId!, (int)enrollment.OrgUnitId!, cancellationToken).WithoutCapturingContext();
                logger.LogInformation("Enrollment was successfully deleted.");

                await commandDispatcher.DispatchAsync(MarkAsHandled.Successfully(eventId, enrollmentId), cancellationToken).WithoutCapturingContext();
            }
            catch (ApiException exception)
            {
                logger.LogWarning(exception, "Error while deleting enrollment. {ErrorMessage}", exception.GetErrorMessage());

                await commandDispatcher.DispatchAsync(MarkAsHandled.Unsuccessfully(eventId, enrollmentId, exception.GetErrorMessage()), cancellationToken).WithoutCapturingContext();
            }
        }
    }
}
