using HR.BrightspaceConnector.Features.Users;
using HR.BrightspaceConnector.Infrastructure.Hosting;
using HR.Common.Utilities;

using Microsoft.EntityFrameworkCore;

namespace HR.BrightspaceConnector.Infrastructure.Persistence
{
    public class Database : IDatabase
    {
        private readonly BrightspaceDbContext dbContext;
        private readonly IHostEnvironment environment;
        private readonly ILogger logger;

        public Database(BrightspaceDbContext dbContext, IHostEnvironment environment, ILogger<Database> logger)
        {
            this.dbContext = dbContext;
            this.environment = environment;
            this.logger = logger;
        }

        public async Task<UserRecord?> GetNextUserAsync(CancellationToken cancellationToken = default)
        {
            var sprocName = string.Format("sync_out_brightspace_{0}_user_GetNextEvents", environment.GetStoredProcedureEnvironmentName());
            logger.LogDebug("Executing stored procedure '{SprocName}'.", sprocName);

            var users = await dbContext.Users.FromSqlRaw(sprocName).AsNoTracking().ToListAsync(cancellationToken).WithoutCapturingContext();
            return users.SingleOrDefault();
        }

        public async Task MarkAsHandledAsync(
            int eventId,
            bool success,
            int? id,
            string? message,
            CancellationToken cancellationToken = default)
        {
            logger.LogDebug("Executing stored procedure 'sync_event_MarkHandled'.");

            await dbContext.Database.ExecuteSqlInterpolatedAsync($"sync_event_MarkHandled {eventId}, {success}, {id?.ToString()}, {message}", cancellationToken).WithoutCapturingContext();
        }
    }
}
