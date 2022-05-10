using HR.BrightspaceConnector.Features.Common.Commands;
using HR.BrightspaceConnector.Utilities;
using HR.Common.Cqrs.Commands;
using HR.Common.Utilities;

namespace HR.BrightspaceConnector.Features.Users.Commands
{
    public class DeleteUser : ICommand
    {
        public DeleteUser(UserRecord user)
        {
            User = user;
        }

        public UserRecord User { get; }
    }

    public class DeleteUserHandler : ICommandHandler<DeleteUser>
    {
        private readonly IApiClient apiClient;
        private readonly ICommandDispatcher commandDispatcher;
        private readonly ILogger logger;

        public DeleteUserHandler(IApiClient apiClient, ICommandDispatcher commandDispatcher, ILogger<DeleteUser> logger)
        {
            this.apiClient = apiClient;
            this.commandDispatcher = commandDispatcher;
            this.logger = logger;
        }

        public async Task HandleAsync(DeleteUser command, CancellationToken cancellationToken)
        {
            UserRecord user = command.User;
            int userId = Convert.ToInt32(user.SyncExternalKey);
            int eventId = (int)user.SyncEventId!;

            logger.LogInformation("Deleting user with username \"{UserName}\" from Brightspace.", user.UserName);

            try
            {
                await apiClient.DeleteUserAsync(userId, cancellationToken).WithoutCapturingContext();
                logger.LogInformation("User was successfully deleted.");

                await commandDispatcher.DispatchAsync(MarkAsHandled.Successfully(eventId, userId), cancellationToken).WithoutCapturingContext();
            }
            catch (ApiException exception)
            {
                logger.LogWarning(exception, "Error while deleting user. {ErrorMessage}", exception.GetErrorMessage());

                await commandDispatcher.DispatchAsync(MarkAsHandled.Unsuccessfully(eventId, userId, exception.GetErrorMessage()), cancellationToken).WithoutCapturingContext();
            }
        }
    }
}
