using System.ComponentModel.DataAnnotations;

namespace HR.BrightspaceConnector.Security
{
    public class OAuthSettings
    {
        [Required]
        public string? ClientId { get; set; }

        [Required]
        public string? ClientSecret { get; set; }

        /// <summary>
        /// URI containing the address of the OAuth token endpoint.
        /// </summary>
        [Required]
        public Uri? TokenEndpoint { get; set; }

        /// <summary>
        /// Fully qualified path to the file in which the OAuth token is cached.
        /// </summary>
        [Required]
        public string? TokenCacheFile { get; set; }

        /// <summary>
        /// The initial refresh token.
        /// </summary>
        [Required]
        public string? RefreshToken { get; set; }

        /// <summary>
        /// Determines how much earlier a token should be considered expired than the actual expiration date.
        /// Valid range of values is between 00:00:00 (none) and 1.00:00:00 (1 day).
        /// </summary>
        [Range(typeof(TimeSpan), "00:00:00", "1.00:00:00")]
        public TimeSpan ExpirationDelta { get; set; } = TimeSpan.Zero;
    }
}
