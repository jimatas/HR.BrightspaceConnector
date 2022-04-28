namespace HR.BrightspaceConnector.Utilities
{
    /// <summary>
    /// Default implementation of the <see cref="IClock"/> interface that simply returns the current time on the computer.
    /// </summary>
    public class SystemClock : IClock
    {
        /// <summary>
        /// Default shared instance of the <see cref="SystemClock"/> class.
        /// </summary>
        public static readonly IClock Instance = new SystemClock();

        /// <inheritdoc/>
        public DateTimeOffset Now => DateTimeOffset.Now;

        /// <inheritdoc/>
        public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
    }
}
