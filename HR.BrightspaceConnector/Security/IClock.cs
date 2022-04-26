namespace HR.BrightspaceConnector.Security
{
    /// <summary>
    /// Abstracts the system clock for testing purposes.
    /// </summary>
    public interface IClock
    {
        /// <summary>
        /// Gets a <see cref="DateTimeOffset"/> object representing the current local time on the system.
        /// </summary>
        DateTimeOffset Now { get; }

        /// <summary>
        /// Gets a <see cref="DateTimeOffset"/> object representing the current UTC time.
        /// </summary>
        DateTimeOffset UtcNow { get; }
    }
}
