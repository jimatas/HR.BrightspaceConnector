using HR.BrightspaceConnector.Features.Common.Commands;
using HR.BrightspaceConnector.Utilities;
using HR.Common.Cqrs.Commands;
using HR.Common.Utilities;

namespace HR.BrightspaceConnector.Features.Enrollments.Commands
{
    public class UpdateEnrollment : ICommand
    {
        public UpdateEnrollment(EnrollmentRecord enrollment)
        {
            Enrollment = enrollment;
        }

        public EnrollmentRecord Enrollment { get; }
    }

    public class UpdateEnrollmentHandler : ICommandHandler<UpdateEnrollment>
    {
        private readonly IApiClient apiClient;
        private readonly ICommandDispatcher commandDispatcher;
        private readonly ILogger logger;

        public UpdateEnrollmentHandler(IApiClient apiClient, ICommandDispatcher commandDispatcher, ILogger<UpdateEnrollment> logger)
        {
            this.apiClient = apiClient;
            this.commandDispatcher = commandDispatcher;
            this.logger = logger;
        }

        public async Task HandleAsync(UpdateEnrollment command, CancellationToken cancellationToken)
        {
            EnrollmentRecord enrollment = command.Enrollment;
            int eventId = (int)enrollment.SyncEventId!;
            _ = long.TryParse(enrollment.SyncExternalKey, out var enrollmentId);

            logger.LogInformation("Updating enrollment of user with ID {UserId} in org unit with ID {OrgUnitId} in Brightspace.", enrollment.UserId, enrollment.OrgUnitId);

            try
            {
                await apiClient.CreateOrUpdateEnrollmentAsync(enrollment.ToCreateEnrollmentData(), cancellationToken).WithoutCapturingContext();
                logger.LogInformation("Enrollment was successfully updated.");

                await commandDispatcher.DispatchAsync(MarkAsHandled.Successfully(eventId, enrollmentId), cancellationToken).WithoutCapturingContext();
            }
            catch (ApiException exception)
            {
                logger.LogWarning(exception, "Error while updating enrollment. {ErrorMessage}", exception.GetErrorMessage());

                await commandDispatcher.DispatchAsync(MarkAsHandled.Unsuccessfully(eventId, enrollmentId, exception.GetErrorMessage()), cancellationToken).WithoutCapturingContext();
            }
        }
    }
}
