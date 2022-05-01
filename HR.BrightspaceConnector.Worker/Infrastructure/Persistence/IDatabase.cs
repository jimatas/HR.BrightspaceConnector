using HR.BrightspaceConnector.Features.Users;

namespace HR.BrightspaceConnector.Infrastructure.Persistence
{
    /// <summary>
    /// Abstracts the source database and the stored procedures through which it is accessed.
    /// </summary>
    public interface IDatabase
    {
        Task<UserRecord?> GetNextUserAsync(CancellationToken cancellationToken = default);

        Task MarkAsHandledAsync(
            bool success,
            string? message,
            int? id,
            int? eventId,
            CancellationToken cancellationToken = default);
    }
}
