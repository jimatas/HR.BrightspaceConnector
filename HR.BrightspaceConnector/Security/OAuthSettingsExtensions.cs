namespace HR.BrightspaceConnector.Security
{
    internal static class OAuthSettingsExtensions
    {
        public static IDictionary<string, string?> AsFormData(this OAuthSettings settings)
        {
            return new Dictionary<string, string?>
            {
                { "grant_type", "refresh_token" },
                { "refresh_token", settings.RefreshToken },
                { "client_id", settings.ClientId },
                { "client_secret", settings.ClientSecret }
            };
        }

        public static OAuthSettings Clone(this OAuthSettings settings)
        {
            return new()
            {
                ClientId = settings.ClientId,
                ClientSecret = settings.ClientSecret,
                ExpirationDelta = settings.ExpirationDelta,
                RefreshToken = settings.RefreshToken,
                TokenCacheFilePath = settings.TokenCacheFilePath,
                TokenEndpointUrl = settings.TokenEndpointUrl
            };
        }
    }
}
