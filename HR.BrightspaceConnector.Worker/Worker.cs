using HR.BrightspaceConnector.Features.OrgUnits.Commands;
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

        public Worker(IDispatcher dispatcher, IOptions<BatchSettings> batchSettings, ILogger<Worker> logger)
        {
            this.dispatcher = dispatcher;
            this.batchSettings = batchSettings.Value;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                var isDeleteContext = false;
                while (!stoppingToken.IsCancellationRequested)
                {
                    logger.LogInformation("Starting new batch run.");

                    await dispatcher.DispatchAsync(new ProcessUsers(batchSettings.BatchSize, isDeleteContext), stoppingToken).WithoutCapturingContext();
                    await dispatcher.DispatchAsync(new ProcessOrgUnits(batchSettings.BatchSize, isDeleteContext), stoppingToken).WithoutCapturingContext();
                    isDeleteContext = !isDeleteContext;

                    logger.LogInformation("Done running batch.");

                    await Task.Delay(batchSettings.TimeDelayBetweenRuns, stoppingToken).WithoutCapturingContext();
                }
            }
            catch (TaskCanceledException)
            {
                logger.LogWarning("The BackgroundService is stopping because a task was canceled.");
            }
            catch (Exception exception)
            {
                logger.LogCritical(exception, "The BackgroundService failed.");

                Environment.Exit(exception.HResult);
            }
        }
    }
}
