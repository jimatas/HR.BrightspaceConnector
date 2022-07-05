using HR.BrightspaceConnector.Features.Courses.Commands;
using HR.BrightspaceConnector.Features.OrgUnits.Commands;
using HR.BrightspaceConnector.Features.Users.Commands;
using HR.BrightspaceConnector.Infrastructure;
using HR.Common.Cqrs.Commands;
using HR.Common.Utilities;

using Microsoft.Extensions.Options;

namespace HR.BrightspaceConnector
{
    public class Worker : BackgroundService
    {
        private readonly ICommandDispatcher commandDispatcher;
        private readonly BatchSettings batchSettings;
        private readonly ILogger logger;

        public Worker(ICommandDispatcher commandDispatcher, IOptions<BatchSettings> batchSettings, ILogger<Worker> logger)
        {
            this.commandDispatcher = commandDispatcher;
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

                    await commandDispatcher.DispatchAsync(new ProcessUsers(batchSettings.BatchSize, isDeleteContext), stoppingToken).WithoutCapturingContext();
                    await commandDispatcher.DispatchAsync(new ProcessOrgUnits(batchSettings.BatchSize, isDepartmentType: false, isDeleteContext), stoppingToken).WithoutCapturingContext();
                    await commandDispatcher.DispatchAsync(new ProcessOrgUnits(batchSettings.BatchSize, isDepartmentType: true, isDeleteContext), stoppingToken).WithoutCapturingContext();
                    await commandDispatcher.DispatchAsync(new ProcessCourseTemplates(batchSettings.BatchSize, isDeleteContext), stoppingToken).WithoutCapturingContext();

                    logger.LogInformation("Done running batch.");

                    await Task.Delay(batchSettings.TimeDelayBetweenRuns, stoppingToken).WithoutCapturingContext();
                    isDeleteContext = !isDeleteContext;
                }
            }
            catch (TaskCanceledException)
            {
                logger.LogWarning("The BackgroundService is stopping because a task was canceled.");
            }
            catch (Exception exception)
            {
                logger.LogCritical(exception, "The BackgroundService failed with the following error: {ExceptionMessage}", exception.Message);

                Environment.Exit(exception.HResult);
            }
        }
    }
}
