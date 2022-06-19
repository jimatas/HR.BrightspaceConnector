using System.ComponentModel.DataAnnotations;

namespace HR.BrightspaceConnector.Infrastructure
{
    public class RecoverySettings
    {
        public static class Names
        {
            public const string CommandTimeoutExpired = nameof(CommandTimeoutExpired);
            public const string TransientHttpFault = nameof(TransientHttpFault);
        }

        /// <summary>
        /// The number of attempts to retry the failed/faulted action.
        /// Default is 4 attempts.
        /// </summary>
        [Range(0, int.MaxValue, ErrorMessage = "The field {0} must be greater than or equal to {1}.")]
        public int RetryAttempts { get; set; } = 4;

        /// <summary>
        /// The time delay before a retry attempt.
        /// Default value is 15 seconds.
        /// Valid range of values is between 00:00:00 (no delay) and 1.00:00:00 (1 day).
        /// </summary>
        [Range(typeof(TimeSpan), "00:00:00", "1.00:00:00")]
        public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(15);

        /// <summary>
        /// The rate at which the <see cref="RetryDelay"/> grows with each subsequent attempt.
        /// Default value is 1.0 (no growth).
        /// Minimum value is 0.0 (no additional attempts).
        /// </summary>
        [Range(0.0, double.MaxValue, ErrorMessage = "The field {0} must be greater than or equal to {1}.")]
        public double BackOffRate { get; set; } = 1.0;
    }
}
