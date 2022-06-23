using HR.BrightspaceConnector.Features.OrgUnits;
using HR.BrightspaceConnector.Features.Users;

namespace HR.BrightspaceConnector.Infrastructure.Persistence
{
    /// <summary>
    /// Abstracts the source database and the stored procedures through which it is accessed.
    /// </summary>
    public interface IDatabase
    {
        Task<UserRecord?> GetNextUserAsync(CancellationToken cancellationToken = default);
        Task<OrgUnitRecord?> GetNextCustomOrgUnitAsync(CancellationToken cancellationToken = default);
        Task<OrgUnitRecord?> GetNextDepartmentAsync(CancellationToken cancellationToken = default);

        Task MarkAsHandledAsync(
            int eventId,
            bool success,
            int? id,
            string? message,
            CancellationToken cancellationToken = default);
    }
}
