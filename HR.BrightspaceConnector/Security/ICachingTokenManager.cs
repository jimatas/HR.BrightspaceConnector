using HR.Common.Utilities;

namespace HR.BrightspaceConnector.Security
{
    public interface ICachingTokenManager : ITokenManager
    {
        bool HasExpired(CacheableTokenResponse cacheableTokenResponse);
        Task<AsyncOutResult<bool, CacheableTokenResponse?>> ReadFromCacheAsync(CancellationToken cancellationToken = default);
        Task StoreInCacheAsync(CacheableTokenResponse cacheableTokenResponse, CancellationToken cancellationToken = default);
    }
}
