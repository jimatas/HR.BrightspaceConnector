using HR.BrightspaceConnector.Features.Users.Commands;
using HR.Common.Cqrs;
using HR.Common.Utilities;

using Microsoft.Extensions.Options;

namespace HR.BrightspaceConnector
{
    public class Worker : BackgroundService
    {
        private readonly IDispatcher dispatcher;
        private readonly BatchSettings batchSettings;
        private readonly ILogger logger;
        private readonly IApiClient apiClient;

        public Worker(IDispatcher dispatcher, IOptions<BatchSettings> batchSettings, ILogger<Worker> logger, IApiClient apiClient)
        {
            this.dispatcher = dispatcher;
            this.batchSettings = batchSettings.Value;
            this.logger = logger;
            this.apiClient = apiClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {       
            var isDeleteContext = false;
            while (!stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation("Starting new batch run.");

                await dispatcher.DispatchAsync(new ProcessUsers(batchSettings.BatchSize, isDeleteContext), stoppingToken).WithoutCapturingContext();
                isDeleteContext = !isDeleteContext;

                logger.LogInformation("Done running batch.");

                await Task.Delay(batchSettings.TimeDelayBetweenRuns, stoppingToken).WithoutCapturingContext();
            }
        }
    }
}
