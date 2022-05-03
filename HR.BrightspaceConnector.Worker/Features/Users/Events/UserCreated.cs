using HR.BrightspaceConnector.Features.Common.Commands;
using HR.BrightspaceConnector.Utilities;
using HR.Common.Cqrs.Commands;
using HR.Common.Cqrs.Events;
using HR.Common.Utilities;

namespace HR.BrightspaceConnector.Features.Users.Events
{
    public class UserCreated : IEvent
    {
        public UserCreated(int eventId, UserData userData)
        {
            Success = true;
            EventId = eventId;
            UserId = userData.UserId;
        }

        public UserCreated(int eventId, ApiException exception)
        {
            EventId = eventId;
            ErrorMessage = exception.GetErrorMessage();
        }

        public bool Success { get; }
        public int EventId { get; }
        public int? UserId { get; }
        public string? ErrorMessage { get; }
    }

    public class UserCreatedHandler : IEventHandler<UserCreated>
    {
        private readonly ICommandDispatcher commandDispatcher;
        private readonly ILogger logger;

        public UserCreatedHandler(ICommandDispatcher commandDispatcher, ILogger<UserCreated> logger)
        {
            this.commandDispatcher = commandDispatcher;
            this.logger = logger;
        }

        public async Task HandleAsync(UserCreated e, CancellationToken cancellationToken)
        {
            logger.LogDebug($"Handling {nameof(UserCreated)} event by dispatching {nameof(MarkAsHandled)} command.");

            await commandDispatcher.DispatchAsync(
                new MarkAsHandled(
                    e.EventId,
                    e.Success,
                    e.UserId,
                    e.ErrorMessage),
                cancellationToken).WithoutCapturingContext();
        }
    }
}
