using HR.BrightspaceConnector.Infrastructure.Persistence;
using HR.BrightspaceConnector.Utilities;
using HR.Common.Cqrs.Events;
using HR.Common.Utilities;

namespace HR.BrightspaceConnector.Features.Users.Events
{
    public class UserCreated : IEvent
    {
        public UserCreated(UserRecord user, UserData userData)
        {
            User = user;
            UserData = userData;
        }

        public UserCreated(UserRecord user, ApiException exception)
        {
            User = user;
            Exception = exception;
        }

        public UserRecord User { get; }

        /// <summary>
        /// If the user was successfully created in Brightspace, this property will hold the user data block that was returned by the server containing the user's generated ID.
        /// </summary>
        public UserData? UserData { get; }

        /// <summary>
        /// If user creation was unsuccessful, this property will hold the exception that was thrown by the API client.
        /// </summary>
        public ApiException? Exception { get; }
    }

    public class UserCreatedHandler : IEventHandler<UserCreated>
    {
        private readonly IDatabase database;
        private readonly ILogger logger;

        public UserCreatedHandler(IDatabase database, ILogger<UserCreated> logger)
        {
            this.database = database;
            this.logger = logger;
        }

        public async Task HandleAsync(UserCreated e, CancellationToken cancellationToken)
        {
            await database.MarkAsHandledAsync(
                (int)e.User.SyncEventId!,
                e.UserData is not null,
                e.UserData?.UserId,
                e.Exception?.GetErrorMessage(),
                cancellationToken).WithoutCapturingContext();

            logger.LogInformation("Marked User with UserName \"{UserName}\" as handled in database. "
                + "Success = {Success}, EventId = {EventId}.", e.User.UserName, e.UserData is not null, e.User.SyncEventId);
        }
    }
}
