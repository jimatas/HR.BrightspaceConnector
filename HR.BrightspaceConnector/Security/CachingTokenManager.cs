﻿using HR.Common.Utilities;

using Microsoft.Extensions.Options;

using System.Text.Json;

namespace HR.BrightspaceConnector.Security
{
    public class CachingTokenManager : ICachingTokenManager
    {
        private readonly HttpClient httpClient;
        private readonly JsonSerializerOptions jsonOptions;
        private readonly OAuthSettings oAuthSettings;
        private readonly IClock clock;
        private readonly SemaphoreSlim mutex = new(1, 1);

        public CachingTokenManager(
            IHttpClientFactory httpClientFactory,
            IOptions<JsonSerializerOptions> jsonOptions,
            IOptions<OAuthSettings> oAuthSettings,
            IClock clock) : this(httpClientFactory.CreateClient(OAuthSettings.HttpClientName), jsonOptions.Value, oAuthSettings.Value, clock) { }

        public CachingTokenManager(HttpClient httpClient, JsonSerializerOptions jsonOptions, OAuthSettings oAuthSettings, IClock clock)
        {
            this.httpClient = httpClient;
            this.jsonOptions = jsonOptions;
            this.oAuthSettings = oAuthSettings;
            this.clock = clock;
        }

        public async Task<TokenResponse> GetTokenAsync(CancellationToken cancellationToken = default)
        {
            if ((await ReadFromCacheAsync(cancellationToken).WithoutCapturingContext()).Out(out var tokenResponse) && !HasExpired(tokenResponse))
            {
                return tokenResponse;
            }

            if (tokenResponse is not null)
            {
                oAuthSettings.RefreshToken = tokenResponse.RefreshToken;
            }

            tokenResponse = await RefreshTokenAsync(cancellationToken).WithoutCapturingContext();
            await StoreInCacheAsync(tokenResponse, cancellationToken).WithoutCapturingContext();

            return tokenResponse;
        }

        public bool HasExpired(CacheableTokenResponse cacheableTokenResponse)
        {
            return clock.Now + oAuthSettings.ExpirationDelta > cacheableTokenResponse.ExpiresAt;
        }

        public async Task<AsyncOutResult<bool, CacheableTokenResponse?>> ReadFromCacheAsync(CancellationToken cancellationToken = default)
        {
            CacheableTokenResponse? tokenResponse = null;
            var serializedTokenData = string.Empty;
            var tokenCacheFilePath = oAuthSettings.TokenCacheFilePath ?? throw new InvalidOperationException("No cache file for OAuth token storage configured.");

            using (await mutex.WaitAndReleaseAsync(cancellationToken).WithoutCapturingContext())
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

        public async Task StoreInCacheAsync(CacheableTokenResponse cacheableTokenResponse, CancellationToken cancellationToken = default)
        {
            var serializedTokenData = JsonSerializer.Serialize(cacheableTokenResponse, jsonOptions);
            var tokenCacheFilePath = oAuthSettings.TokenCacheFilePath ?? throw new InvalidOperationException("No cache file for OAuth token storage configured.");

            using (await mutex.WaitAndReleaseAsync(cancellationToken).WithoutCapturingContext())
            {
                new FileInfo(tokenCacheFilePath).Directory?.Create();
                await File.WriteAllTextAsync(tokenCacheFilePath, serializedTokenData, cancellationToken).WithoutCapturingContext();
            }
        }

        private async Task<CacheableTokenResponse> RefreshTokenAsync(CancellationToken cancellationToken)
        {
            using var httpResponse = await httpClient.PostAsync(string.Empty, new FormUrlEncodedContent(oAuthSettings.AsFormData()), cancellationToken).WithoutCapturingContext();
            var jsonData = await httpResponse.Content.ReadAsStringAsync(cancellationToken).WithoutCapturingContext();

            if (httpResponse.IsSuccessStatusCode)
            {
                var tokenResponse = JsonSerializer.Deserialize<CacheableTokenResponse>(jsonData, jsonOptions);
                return tokenResponse!;
            }

            var errorMessage = $"HTTP {(int)httpResponse.StatusCode} - {httpResponse.ReasonPhrase}";
            var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(jsonData, jsonOptions);
            if (!(string.IsNullOrEmpty(errorResponse?.ErrorCode) || string.IsNullOrEmpty(errorResponse.ErrorDescription)))
            {
                errorMessage = $"{errorResponse.ErrorDescription} ({errorResponse.ErrorCode})";
            }
            throw new InvalidOperationException(errorMessage);
        }
    }
}