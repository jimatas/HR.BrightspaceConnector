namespace HR.BrightspaceConnector.Security
{
    public interface ICachingTokenManager : ITokenManager
    {
        Task<AsyncOutResult<bool, CacheableTokenResponse?>> TryGetTokenFromCacheAsync(CancellationToken cancellationToken = default);
        Task StoreTokenInCacheAsync(CacheableTokenResponse cacheableTokenResponse, CancellationToken cancellationToken = default);
    }
}
