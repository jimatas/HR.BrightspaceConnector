using HR.BrightspaceConnector.Security;
using HR.Common.Utilities;

namespace HR.BrightspaceConnector
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> logger;
        private readonly ITokenManager tokenManager;

        public Worker(ILogger<Worker> logger, ITokenManager tokenManager)
        {
            this.logger = logger;
            this.tokenManager = tokenManager;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Starting new batch run.");

            var token = await tokenManager.GetTokenAsync(stoppingToken).WithoutCapturingContext();

            //await commandDispatcher.DispatchAsync(new ProcessUsers(), stoppingToken).WithoutCapturingContext();
            //await commandDispatcher.DispatchAsync(new ProcessUsers(isDeleteContext: true), stoppingToken).WithoutCapturingContext();

            logger.LogInformation("Done running batch.");
        }
    }
}
