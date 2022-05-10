using HR.BrightspaceConnector.Features.Common.Commands;
using HR.BrightspaceConnector.Utilities;
using HR.Common.Cqrs.Commands;
using HR.Common.Utilities;

namespace HR.BrightspaceConnector.Features.Users.Commands
{
    public class CreateUser : ICommand
    {
        public CreateUser(UserRecord user)
        {
            User = user;
        }

        public UserRecord User { get; }
    }

    public class CreateUserHandler : ICommandHandler<CreateUser>
    {
        private readonly IApiClient apiClient;
        private readonly ICommandDispatcher commandDispatcher;
        private readonly ILogger logger;

        public CreateUserHandler(IApiClient apiClient, ICommandDispatcher commandDispatcher, ILogger<CreateUser> logger)
        {
            this.apiClient = apiClient;
            this.commandDispatcher = commandDispatcher;
            this.logger = logger;
        }

        public async Task HandleAsync(CreateUser command, CancellationToken cancellationToken)
        {
            UserRecord user = command.User;
            int eventId = (int)user.SyncEventId!;

            logger.LogInformation("Creating user with username \"{UserName}\" in Brightspace.", user.UserName);

            try
            {
                var userData = await apiClient.CreateUserAsync(user.ToCreateUserData(), cancellationToken).WithoutCapturingContext();
                logger.LogInformation("User was successfully created.");

                await commandDispatcher.DispatchAsync(MarkAsHandled.Successfully(eventId, (int)userData.UserId!), cancellationToken).WithoutCapturingContext();
            }
            catch (ApiException exception)
            {
                logger.LogWarning(exception, "Error while creating user. {ErrorMessage}", exception.GetErrorMessage());

                await commandDispatcher.DispatchAsync(MarkAsHandled.Unsuccessfully(eventId, exception.GetErrorMessage()), cancellationToken).WithoutCapturingContext();
            }
        }
    }
}
