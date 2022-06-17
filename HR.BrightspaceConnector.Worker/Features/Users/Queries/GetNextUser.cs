using HR.BrightspaceConnector.Infrastructure.Persistence;
using HR.Common.Cqrs.Queries;
using HR.Common.Utilities;

namespace HR.BrightspaceConnector.Features.Users.Queries
{
    public class GetNextUser : IQuery<UserRecord?>
    {
    }

    public class GetNextUserHandler : IQueryHandler<GetNextUser, UserRecord?>
    {
        private readonly IDatabase database;
        private readonly ILogger logger;

        public GetNextUserHandler(IDatabase database, ILogger<GetNextUser> logger)
        {
            this.database = database;
            this.logger = logger;
        }

        public async Task<UserRecord?> HandleAsync(GetNextUser query, CancellationToken cancellationToken)
        {
            var user = await database.GetNextUserAsync(cancellationToken).WithoutCapturingContext();
            if (user is not null)
            {
                logger.LogInformation("Retrieved user with username \"{UserName}\" for sync action '{SyncAction}' from database.", user.UserName, user.SyncAction);
            }

            return user;
        }
    }
}
