using HR.BrightspaceConnector.Infrastructure;
using HR.BrightspaceConnector.Utilities;
using HR.Common.Cqrs;
using HR.Common.Cqrs.Queries;
using HR.Common.Utilities;

using Microsoft.Extensions.Options;

namespace HR.BrightspaceConnector.Features.Common.Wrappers
{
    public class HandleQueryWithRetry<TQuery, TResult> : IQueryHandlerWrapper<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        private readonly RecoverySettings recoverySettings;
        private readonly ILogger logger;

        public HandleQueryWithRetry(IOptionsSnapshot<RecoverySettings> recoverySettings, ILogger<HandleQueryWithRetry<TQuery, TResult>> logger)
        {
            this.recoverySettings = recoverySettings.Get(RecoverySettings.Names.CommandTimeoutExpired);
            this.logger = logger;
        }

        public async Task<TResult> HandleAsync(TQuery query, HandlerDelegate<TResult> next, CancellationToken cancellationToken)
        {
            return await HandleWithRetryAsync(query, next, recoverySettings.RetryAttempts, cancellationToken).WithoutCapturingContext();
        }

        private async Task<TResult> HandleWithRetryAsync(TQuery query, HandlerDelegate<TResult> next, int retries, CancellationToken cancellationToken)
        {
            try
            {
                return await next().WithoutCapturingContext();
            }
            catch (Exception ex) when (ex.IsTimeoutException() && retries > 0)
            {
                var retryDelay = recoverySettings.CalculateRetryDelay(recoverySettings.RetryAttempts - (retries - 1));

                logger.LogWarning("Timeout expired waiting for {Query} query to finish. Attempting retry in {RetryDelay}", query.GetType().Name, retryDelay);

                await Task.Delay(retryDelay, cancellationToken).WithoutCapturingContext();
                return await HandleWithRetryAsync(query, next, retries - 1, cancellationToken).WithoutCapturingContext();
            }
        }
    }
}
