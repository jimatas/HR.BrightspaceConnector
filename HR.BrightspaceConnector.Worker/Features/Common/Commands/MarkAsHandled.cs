using HR.BrightspaceConnector.Infrastructure.Persistence;
using HR.Common.Cqrs.Commands;
using HR.Common.Utilities;

namespace HR.BrightspaceConnector.Features.Common.Commands
{
    public class MarkAsHandled : ICommand
    {
        public MarkAsHandled(int? eventId, bool success, int? id, string? errorMessage)
        {
            EventId = eventId;
            Success = success;
            Id = id;
            ErrorMessage = errorMessage;
        }

        public int? EventId { get; }
        public bool Success { get; }

        /// <summary>
        /// The unique ID of the object in Brightspace. 
        /// The value is generated upon successful creation of the object and maps to SyncExternalKey.
        /// </summary>
        public int? Id { get; }

        /// <summary>
        /// Either a HTTP status message that describes an error situation, or any (validation) errors that were returned by the server, flattened to a single error message.
        /// </summary>
        public string? ErrorMessage { get; }
    }

    public class MarkAsHandledHandler : ICommandHandler<MarkAsHandled>
    {
        private readonly IDatabase database;
        private readonly ILogger logger;

        public MarkAsHandledHandler(IDatabase database, ILogger<MarkAsHandled> logger)
        {
            this.database = database;
            this.logger = logger;
        }

        public async Task HandleAsync(MarkAsHandled command, CancellationToken cancellationToken)
        {
            await database.MarkAsHandledAsync(
                command.EventId,
                command.Success,
                command.Id,
                command.ErrorMessage,
                cancellationToken).WithoutCapturingContext();

            logger.LogInformation("Marked object with Id {Id} as handled in database.", command.Id?.ToString() ?? "[n/a]");
        }
    }
}
