namespace HR.BrightspaceConnector.Security
{
    public static class OAuthSettingsExtensions
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
    }
}
