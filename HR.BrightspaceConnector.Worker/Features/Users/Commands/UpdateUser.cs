using HR.BrightspaceConnector.Features.Common.Commands;
using HR.BrightspaceConnector.Utilities;
using HR.Common.Cqrs.Commands;
using HR.Common.Utilities;

namespace HR.BrightspaceConnector.Features.Users.Commands
{
    public class UpdateUser : ICommand
    {
        public UpdateUser(UserRecord user)
        {
            User = user;
        }

        public UserRecord User { get; }
    }

    public class UpdateUserHandler : ICommandHandler<UpdateUser>
    {
        private readonly IApiClient apiClient;
        private readonly ICommandDispatcher commandDispatcher;
        private readonly ILogger logger;

        public UpdateUserHandler(IApiClient apiClient, ICommandDispatcher commandDispatcher, ILogger<UpdateUser> logger)
        {
            this.apiClient = apiClient;
            this.commandDispatcher = commandDispatcher;
            this.logger = logger;
        }

        public async Task HandleAsync(UpdateUser command, CancellationToken cancellationToken)
        {
            UserRecord user = command.User;
            int userId = Convert.ToInt32(user.SyncExternalKey);
            int eventId = (int)user.SyncEventId!;

            logger.LogInformation("Updating user with username \"{UserName}\" in Brightspace.", user.UserName);

            try
            {
                var userData = await apiClient.UpdateUserAsync(userId, user.ToUpdateUserData(), cancellationToken).WithoutCapturingContext();
                logger.LogInformation("User was successfully updated.");

                await commandDispatcher.DispatchAsync(MarkAsHandled.Successfully(eventId, userId), cancellationToken).WithoutCapturingContext();
            }
            catch (ApiException exception)
            {
                logger.LogWarning(exception, "Error while updating user. {ErrorMessage}", exception.GetErrorMessage());

                await commandDispatcher.DispatchAsync(MarkAsHandled.Unsuccessfully(eventId, userId, exception.GetErrorMessage()), cancellationToken).WithoutCapturingContext();
            }
        }
    }
}
