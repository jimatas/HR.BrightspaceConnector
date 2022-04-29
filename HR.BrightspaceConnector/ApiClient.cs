using HR.BrightspaceConnector.Features.Roles;
using HR.BrightspaceConnector.Security;
using HR.Common.Utilities;

using Microsoft.Extensions.Options;

using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace HR.BrightspaceConnector
{
    public class ApiClient : IApiClient
    {
        private readonly HttpClient httpClient;
        private readonly JsonSerializerOptions jsonOptions;
        private readonly ApiSettings apiSettings;
        private readonly ITokenManager tokenManager;

        public ApiClient(HttpClient httpClient, IOptions<JsonSerializerOptions> jsonOptions, IOptions<ApiSettings> apiSettings, ITokenManager tokenManager)
        {
            this.httpClient = httpClient;
            this.jsonOptions = jsonOptions.Value;
            this.apiSettings = apiSettings.Value;
            this.tokenManager = tokenManager;
        }

        public async Task<IEnumerable<Role>> GetRolesAsync(CancellationToken cancellationToken = default)
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"lp/{apiSettings.LearningPlatformVersion}/roles/");
            await SetAuthorizationHeader(httpRequest, cancellationToken).WithoutCapturingContext();

            using var httpResponse = await httpClient.SendAsync(httpRequest, cancellationToken).WithoutCapturingContext();
            var roles = await httpResponse.Content.ReadFromJsonAsync<IEnumerable<Role>>(jsonOptions, cancellationToken).WithoutCapturingContext();

            return roles!;
        }

        private async Task SetAuthorizationHeader(HttpRequestMessage httpRequest, CancellationToken cancellationToken)
        {
            var oAuthToken = await tokenManager.GetTokenAsync(cancellationToken).WithoutCapturingContext();
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue(oAuthToken.TokenType ?? TokenResponse.DefaultTokenType, oAuthToken.AccessToken);
        }
    }
}
