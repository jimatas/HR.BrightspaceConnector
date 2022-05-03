using HR.BrightspaceConnector.Features.Users;
using HR.Common.Utilities;

using Microsoft.EntityFrameworkCore;

namespace HR.BrightspaceConnector.Infrastructure.Persistence
{
    public class Database : IDatabase
    {
        private readonly BrightspaceDbContext dbContext;
        public Database(BrightspaceDbContext dbContext) => this.dbContext = dbContext;

        public async Task<UserRecord?> GetNextUserAsync(CancellationToken cancellationToken = default)
        {
            var users = await dbContext.Users.FromSqlRaw("sync_brightspace_prod_user_GetNextEvents").AsNoTracking().ToListAsync(cancellationToken).WithoutCapturingContext();
            return users.SingleOrDefault();
        }

        public async Task MarkAsHandledAsync(int? eventId, bool success, int? id, string? message, CancellationToken cancellationToken = default)
        {
            await dbContext.Database.ExecuteSqlInterpolatedAsync($"sync_event_MarkHandled {eventId}, {success}, {id?.ToString()}, {message}", cancellationToken).WithoutCapturingContext();
        }
    }
}
