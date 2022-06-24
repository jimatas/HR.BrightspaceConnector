using System.ComponentModel.DataAnnotations;

namespace HR.BrightspaceConnector.Infrastructure
{
    public class ApiClientSettings : ApiSettings
    {
        /// <summary>
        /// Configures how long certain results should be kept in the cache by the <see cref="CachingApiClient"/>.
        /// Valid range of values is between 00:00:00 (not cached) and 1.00:00:00 (1 day).
        /// </summary>
        [Range(typeof(TimeSpan), "00:00:00", "1.00:00:00")]
        public TimeSpan? CacheDuration { get; set; }
    }
}
