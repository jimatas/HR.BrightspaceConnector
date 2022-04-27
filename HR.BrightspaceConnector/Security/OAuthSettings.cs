namespace HR.BrightspaceConnector.Security
{
    public class OAuthSettings
    {
        public string? ClientId { get; set; }
        public string? ClientSecret { get; set; }
        public string? TokenEndpointUrl { get; set; }

        /// <summary>
        /// Fully qualified path to the file in which the OAuth tokens are cached.
        /// </summary>
        public string? TokenCacheFilePath { get; set; }

        /// <summary>
        /// The initial refresh token.
        /// </summary>
        public string? RefreshToken { get; set; }

        /// <summary>
        /// Determines how much earlier a token should be considered expired than the actual expiration date.
        /// </summary>
        public TimeSpan ExpirationDelta { get; set; } = TimeSpan.Zero;
    }
}
