using HR.BrightspaceConnector.Utilities;
using HR.Common.Utilities;

using Microsoft.Extensions.Options;

using System.Net.Http.Json;
using System.Net.Mime;
using System.Text.Json;

namespace HR.BrightspaceConnector.Security
{
    public class FileCachingTokenManager : ICachingTokenManager
    {
        private static readonly AsyncReaderWriterLockSlim fileLock = new();
        private readonly HttpClient httpClient;
        private readonly JsonSerializerOptions jsonOptions;
        private readonly OAuthSettings oAuthSettings;
        private readonly IClock clock;
        
        public FileCachingTokenManager(HttpClient httpClient, IOptions<JsonSerializerOptions> jsonOptions, IOptions<OAuthSettings> oAuthSettings, IClock clock)
        {
            this.httpClient = httpClient;
            this.jsonOptions = jsonOptions.Value;
            this.oAuthSettings = oAuthSettings.Value.Clone();
            this.clock = clock;
        }

        public async Task<TokenResponse> GetTokenAsync(CancellationToken cancellationToken = default)
        {
            if ((await TryGetTokenFromCacheAsync(cancellationToken).WithoutCapturingContext()).Out(out var tokenResponse) && !IsTokenExpired(tokenResponse))
            {
                return tokenResponse;
            }

            if (tokenResponse is not null)
            {
                oAuthSettings.RefreshToken = tokenResponse.RefreshToken;
            }

            tokenResponse = await RefreshTokenAsync(cancellationToken).WithoutCapturingContext();
            await StoreTokenInCacheAsync(tokenResponse, cancellationToken).WithoutCapturingContext();

            return tokenResponse;
        }

        public async Task<AsyncOutResult<bool, CacheableTokenResponse?>> TryGetTokenFromCacheAsync(CancellationToken cancellationToken = default)
        {
            CacheableTokenResponse? tokenResponse = null;
            var serializedTokenData = string.Empty;
            var tokenCacheFilePath = oAuthSettings.TokenCacheFile ?? throw new InvalidOperationException("OAuth token cache file not configured.");

            using (await fileLock.EnterReadLockAndExitAsync(cancellationToken).WithoutCapturingContext())
            {
                try
                {
                    serializedTokenData = await File.ReadAllTextAsync(tokenCacheFilePath, cancellationToken).WithoutCapturingContext();
                }
                catch (FileNotFoundException) { }
                catch (DirectoryNotFoundException) { }
            }

            if (!string.IsNullOrEmpty(serializedTokenData))
            {
                tokenResponse = JsonSerializer.Deserialize<CacheableTokenResponse>(serializedTokenData, jsonOptions);
            }

            return (tokenResponse is not null, tokenResponse);
        }

        public async Task StoreTokenInCacheAsync(CacheableTokenResponse cacheableTokenResponse, CancellationToken cancellationToken = default)
        {
            var serializedTokenData = JsonSerializer.Serialize(cacheableTokenResponse, jsonOptions);
            var tokenCacheFilePath = oAuthSettings.TokenCacheFile ?? throw new InvalidOperationException("OAuth token cache file not configured.");

            using (await fileLock.EnterWriteLockAndExitAsync(cancellationToken).WithoutCapturingContext())
            {
                new FileInfo(tokenCacheFilePath).Directory?.Create();
                await File.WriteAllTextAsync(tokenCacheFilePath, serializedTokenData, cancellationToken).WithoutCapturingContext();
            }
        }

        private bool IsTokenExpired(CacheableTokenResponse cacheableTokenResponse)
        {
            return clock.Now + oAuthSettings.ExpirationDelta > cacheableTokenResponse.ExpiresAt;
        }

        private async Task<CacheableTokenResponse> RefreshTokenAsync(CancellationToken cancellationToken)
        {
            using var httpResponse = await httpClient.PostAsync(string.Empty, new FormUrlEncodedContent(oAuthSettings.AsFormData()), cancellationToken).WithoutCapturingContext();
            if (httpResponse.IsSuccessStatusCode)
            {
                var tokenResponse = await httpResponse.Content.ReadFromJsonAsync<CacheableTokenResponse>(jsonOptions, cancellationToken).WithoutCapturingContext();
                return tokenResponse!;
            }

            // Default error message is the HTTP status description.
            var message = httpResponse.ReasonPhrase;

            // Try to obtain a more meaningful one...
            if (httpResponse.Content.Headers.ContentType?.MediaType == MediaTypeNames.Application.Json)
            {
                var errorResponse = await httpResponse.Content.ReadFromJsonAsync<ErrorResponse>(jsonOptions, cancellationToken).WithoutCapturingContext();
                if (!(string.IsNullOrEmpty(errorResponse?.ErrorCode) || string.IsNullOrEmpty(errorResponse.ErrorDescription)))
                {
                    message = $"{errorResponse.ErrorCode}: {errorResponse.ErrorDescription}";
                }
            }

            throw new HttpRequestException(message, inner: null, httpResponse.StatusCode);
        }
    }
}
