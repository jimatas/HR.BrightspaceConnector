using HR.BrightspaceConnector.Features.Common.Commands;
using HR.BrightspaceConnector.Utilities;
using HR.Common.Cqrs.Commands;
using HR.Common.Utilities;

namespace HR.BrightspaceConnector.Features.Enrollments.Commands
{
    public class CreateEnrollment : ICommand
    {
        public CreateEnrollment(EnrollmentRecord enrollment)
        {
            Enrollment = enrollment;
        }

        public EnrollmentRecord Enrollment { get; }
    }

    public class CreateEnrollmentHandler : ICommandHandler<CreateEnrollment>
    {
        private readonly IApiClient apiClient;
        private readonly ICommandDispatcher commandDispatcher;
        private readonly ILogger logger;

        public CreateEnrollmentHandler(IApiClient apiClient, ICommandDispatcher commandDispatcher, ILogger<CreateEnrollment> logger)
        {
            this.apiClient = apiClient;
            this.commandDispatcher = commandDispatcher;
            this.logger = logger;
        }

        public async Task HandleAsync(CreateEnrollment command, CancellationToken cancellationToken)
        {
            EnrollmentRecord enrollment = command.Enrollment;
            int eventId = (int)enrollment.SyncEventId!;
            _ = long.TryParse(enrollment.SyncExternalKey, out var enrollmentId);

            logger.LogInformation("Creating enrollment for user with ID {UserId} in org unit with ID {OrgUnitId} in Brightspace.", enrollment.UserId, enrollment.OrgUnitId);

            try
            {
                var newEnrollment = await apiClient.CreateOrUpdateEnrollmentAsync(enrollment.ToCreateEnrollmentData(), cancellationToken).WithoutCapturingContext();
                logger.LogInformation("Enrollment was successfully created.");

                await commandDispatcher.DispatchAsync(MarkAsHandled.Successfully(eventId, enrollmentId), cancellationToken).WithoutCapturingContext();
            }
            catch (ApiException exception)
            {
                logger.LogWarning(exception, "Error while creating enrollment. {ErrorMessage}", exception.GetErrorMessage());

                await commandDispatcher.DispatchAsync(MarkAsHandled.Unsuccessfully(eventId, exception.GetErrorMessage()), cancellationToken).WithoutCapturingContext();
            }
        }
    }
}
