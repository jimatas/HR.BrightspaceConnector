namespace HR.BrightspaceConnector
{
    public class BatchSettings
    {
        public const int DefaultBatchSize = 10;
        public static readonly TimeSpan DefaultTimeDelayBetweenRuns = TimeSpan.FromSeconds(1);

        /// <summary>
        /// Proposed batch size per resource (e.g., users) and action performed (create + update or delete).
        /// </summary>
        public int BatchSize { get; set; } = DefaultBatchSize;
        public TimeSpan TimeDelayBetweenRuns { get; set; } = DefaultTimeDelayBetweenRuns;
    }
}
