using HR.BrightspaceConnector.Features.Users;
using HR.BrightspaceConnector.Features.Users.Queries;
using HR.BrightspaceConnector.Security;
using HR.Common.Cqrs.Queries;
using HR.Common.Utilities;

using Microsoft.Extensions.Options;

namespace HR.BrightspaceConnector
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> logger;
        private readonly BatchSettings batchSettings;

        // TODO: Remove these.
        private readonly ITokenManager tokenManager;
        private readonly IApiClient apiClient;
        private readonly IQueryDispatcher queryDispatcher;

        public Worker(IOptions<BatchSettings> batchSettings, ILogger<Worker> logger, ITokenManager tokenManager, IApiClient apiClient, IQueryDispatcher queryDispatcher)
        {
            this.batchSettings = batchSettings.Value;
            this.logger = logger;

            // TODO: Remove these.
            this.tokenManager = tokenManager;
            this.apiClient = apiClient;
            this.queryDispatcher = queryDispatcher;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var isDeleteContext = false;
            while (!stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation("Starting new batch run.");

                var user = await queryDispatcher.DispatchAsync(new GetNextUser(), stoppingToken).WithoutCapturingContext();
                /* 
                var users = await apiClient.GetUsersAsync(queryParameters: new UserQueryParameters { Bookmark = null }, stoppingToken).WithoutCapturingContext();
                users = await apiClient.GetUsersAsync(queryParameters: new UserQueryParameters { ExternalEmail = "Demo.Student@d2l.com" }, stoppingToken).WithoutCapturingContext();
                users = await apiClient.GetUsersAsync(queryParameters: new UserQueryParameters { OrgDefinedId = "Demo.Student" }, stoppingToken).WithoutCapturingContext();
                users = await apiClient.GetUsersAsync(queryParameters: new UserQueryParameters { UserName = "Demo.Student" }, stoppingToken).WithoutCapturingContext(); 
                */
                
                // await commandDispatcher.DispatchAsync(new ProcessUsers(batchSettings.BatchSize, isDeleteContext), stoppingToken).WithoutCapturingContext();
                isDeleteContext = !isDeleteContext;

                logger.LogInformation("Done running batch.");

                await Task.Delay(batchSettings.TimeDelayBetweenRuns, stoppingToken).WithoutCapturingContext();
            }
        }
    }
}
