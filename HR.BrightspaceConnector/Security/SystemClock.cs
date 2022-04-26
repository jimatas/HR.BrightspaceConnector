namespace HR.BrightspaceConnector.Security
{
    /// <summary>
    /// Default implementation of the <see cref="IClock"/> interface that simply returns the current time on the computer.
    /// </summary>
    public class SystemClock : IClock
    {
        /// <summary>
        /// Default publicly accessible, static instance of the <see cref="SystemClock"/> class.
        /// </summary>
        public static readonly IClock Instance = new SystemClock();

        /// <inheritdoc/>
        public DateTimeOffset Now => DateTimeOffset.Now;

        /// <inheritdoc/>
        public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
    }
}
