namespace HR.BrightspaceConnector.Security
{
    /// <summary>
    /// Defines the interface for a type that manages the OAuth access and refresh tokens for a secure API client.
    /// </summary>
    public interface ITokenManager
    {
        Task<TokenResponse> GetTokenAsync(CancellationToken cancellationToken = default);
    }
}
