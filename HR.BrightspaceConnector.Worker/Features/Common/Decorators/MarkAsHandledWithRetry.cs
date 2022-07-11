using HR.BrightspaceConnector.Features.Common.Commands;
using HR.BrightspaceConnector.Infrastructure;
using HR.BrightspaceConnector.Utilities;
using HR.Common.Cqrs;
using HR.Common.Cqrs.Commands;
using HR.Common.Utilities;

using Microsoft.Extensions.Options;

namespace HR.BrightspaceConnector.Features.Common.Decorators
{
    public class MarkAsHandledWithRetry : ICommandHandlerWrapper<MarkAsHandled>
    {
        private readonly RecoverySettings recoverySettings;
        private readonly ILogger logger;

        public MarkAsHandledWithRetry(IOptionsSnapshot<RecoverySettings> recoverySettings, ILogger<MarkAsHandledWithRetry> logger)
        {
            this.recoverySettings = recoverySettings.Get(RecoverySettings.Names.CommandTimeoutExpired);
            this.logger = logger;
        }

        public async Task HandleAsync(MarkAsHandled command, HandlerDelegate next, CancellationToken cancellationToken)
        {
            await HandleWithRetryAsync(command, next, recoverySettings.RetryAttempts, cancellationToken).WithoutCapturingContext();
        }

        private async Task HandleWithRetryAsync(MarkAsHandled command, HandlerDelegate next, int retries, CancellationToken cancellationToken)
        {
            try
            {
                await next().WithoutCapturingContext();
            }
            catch (Exception ex) when (ex.IsTimeoutException() && retries > 0)
            {
                var retryDelay = recoverySettings.CalculateRetryDelay(recoverySettings.RetryAttempts - (retries - 1));

                logger.LogWarning("Timeout expired waiting for MarkAsHandled command to finish. Attempting retry in {RetryDelay}", retryDelay);

                await Task.Delay(retryDelay, cancellationToken).WithoutCapturingContext();
                await HandleWithRetryAsync(command, next, retries - 1, cancellationToken).WithoutCapturingContext();
            }
        }
    }
}
