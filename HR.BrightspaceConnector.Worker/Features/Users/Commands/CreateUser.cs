using HR.BrightspaceConnector.Features.Users.Events;
using HR.BrightspaceConnector.Utilities;
using HR.Common.Cqrs.Commands;
using HR.Common.Cqrs.Events;
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
        private readonly IEventDispatcher eventDispatcher;
        private readonly ILogger logger;

        public CreateUserHandler(IApiClient apiClient, IEventDispatcher eventDispatcher, ILogger<CreateUser> logger)
        {
            this.apiClient = apiClient;
            this.eventDispatcher = eventDispatcher;
            this.logger = logger;
        }

        public async Task HandleAsync(CreateUser command, CancellationToken cancellationToken)
        {
            UserRecord user = command.User;
            logger.LogInformation("Creating new User with UserName \"{UserName}\" in Brightspace.", user.UserName);

            try
            {
                var userData = await apiClient.CreateUserAsync(user.ToCreateUserData(), cancellationToken).WithoutCapturingContext();
                await eventDispatcher.DispatchAsync(new UserCreated(user, userData), cancellationToken).WithoutCapturingContext();
            }
            catch (ApiException exception)
            {
                logger.LogWarning(exception.GetErrorMessage());
                await eventDispatcher.DispatchAsync(new UserCreated(user, exception), cancellationToken).WithoutCapturingContext();
            }
        }
    }
}
