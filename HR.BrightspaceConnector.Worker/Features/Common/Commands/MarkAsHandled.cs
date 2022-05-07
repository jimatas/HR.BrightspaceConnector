﻿using HR.BrightspaceConnector.Infrastructure.Persistence;
using HR.Common.Cqrs.Commands;
using HR.Common.Utilities;

namespace HR.BrightspaceConnector.Features.Common.Commands
{
    public class MarkAsHandled : ICommand
    {
        public MarkAsHandled(int eventId, bool success, int? id, string? message)
        {
            EventId = eventId;
            Success = success;
            Id = id;
            Message = message;
        }

        public static MarkAsHandled WithSuccess(int eventId, int id) => new(eventId, success: true, id, message: null);
        public static MarkAsHandled WithoutSuccess(int eventId, string? message = null) => new(eventId, success: true, id: null, message);

        public int EventId { get; }
        public bool Success { get; }

        /// <summary>
        /// The unique ID of the object in Brightspace. 
        /// Its value is generated upon successful creation of the object and maps to SyncExternalKey.
        /// </summary>
        public int? Id { get; }

        /// <summary>
        /// Either a HTTP status message that describes an error situation, or any (validation) errors that were returned by the server, flattened to a single error message.
        /// </summary>
        public string? Message { get; }
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
                command.Message,
                cancellationToken).WithoutCapturingContext();

            logger.LogInformation("Marked sync event with id {EventId} as handled {Success} in database.",
                command.EventId, command.Success ? "successfully" : "unsuccessfully");
        }
    }
}