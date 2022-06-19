namespace HR.BrightspaceConnector.Infrastructure
{
    public static class RecoverySettingsExtensions
    {
        /// <summary>
        /// Calculates the time delay before the specified retry is attempted, taking into account the back-off rate.
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="attempt">The retry attempt to calculate the time delay for. Where 1 is the first retry attempt, 2 is the second, etc.</param>
        /// <returns></returns>
        public static TimeSpan CalculateRetryDelay(this RecoverySettings settings, int attempt)
        {
            var delay = TimeSpan.Zero;
            if (attempt > 0)
            {
                delay = settings.RetryDelay;
                for (var i = 2; i <= Math.Min(attempt, settings.RetryAttempts); i++)
                {
                    delay = delay.Multiply(settings.BackOffRate);
                }
            }
            return delay;
        }
    }
}
