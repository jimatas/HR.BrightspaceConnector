using HR.BrightspaceConnector.Features.Users;
using HR.BrightspaceConnector.Infrastructure;
using HR.BrightspaceConnector.Security;
using HR.Common.Utilities;

using Microsoft.Extensions.Options;

using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Mime;
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
            await CheckErrorResponseAsync(httpResponse, cancellationToken).WithoutCapturingContext();

            var roles = await httpResponse.Content.ReadFromJsonAsync<IEnumerable<Role>>(jsonOptions, cancellationToken).WithoutCapturingContext();
            return roles!;
        }

        public async Task<PagedResultSet<UserData>> GetUsersAsync(UserQueryParameters? queryParameters, CancellationToken cancellationToken = default)
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"lp/{apiSettings.LearningPlatformVersion}/users/{queryParameters?.ToQueryString(jsonOptions.PropertyNamingPolicy)}");
            await SetAuthorizationHeader(httpRequest, cancellationToken).WithoutCapturingContext();

            using var httpResponse = await httpClient.SendAsync(httpRequest, cancellationToken).WithoutCapturingContext();

            var users = await TryParseUserResponseAsync(queryParameters, httpResponse, cancellationToken).WithoutCapturingContext();
            return users!;
        }

        private async Task<PagedResultSet<UserData>> TryParseUserResponseAsync(UserQueryParameters? queryParameters, HttpResponseMessage httpResponse, CancellationToken cancellationToken)
        {
            if (queryParameters is not null)
            {
                if (!string.IsNullOrEmpty(queryParameters.UserName))
                {
                    if (httpResponse.StatusCode == HttpStatusCode.NotFound)
                    {
                        return new PagedResultSet<UserData>();
                    }
                    else if (httpResponse.IsSuccessStatusCode)
                    {
                        var user = await httpResponse.Content.ReadFromJsonAsync<UserData>(jsonOptions, cancellationToken).WithoutCapturingContext();
                        return new PagedResultSet<UserData> { Items = new[] { user! } };
                    }
                    // else, error status received

                    await CheckErrorResponseAsync(httpResponse, cancellationToken).WithoutCapturingContext();
                }

                if (!string.IsNullOrEmpty(queryParameters.OrgDefinedId) || !string.IsNullOrEmpty(queryParameters.ExternalEmail))
                {
                    await CheckErrorResponseAsync(httpResponse, cancellationToken).WithoutCapturingContext();

                    var users = await httpResponse.Content.ReadFromJsonAsync<IEnumerable<UserData>>(jsonOptions, cancellationToken).WithoutCapturingContext();
                    return new PagedResultSet<UserData> { Items = users! };
                }
            }

            await CheckErrorResponseAsync(httpResponse, cancellationToken).WithoutCapturingContext();

            var pagedUsers = await httpResponse.Content.ReadFromJsonAsync<PagedResultSet<UserData>>(jsonOptions, cancellationToken).WithoutCapturingContext();
            return pagedUsers!;
        }

        private async Task SetAuthorizationHeader(HttpRequestMessage httpRequest, CancellationToken cancellationToken)
        {
            var oAuthToken = await tokenManager.GetTokenAsync(cancellationToken).WithoutCapturingContext();
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue(oAuthToken.TokenType ?? TokenResponse.DefaultTokenType, oAuthToken.AccessToken);
        }

        private async Task CheckErrorResponseAsync(HttpResponseMessage httpResponse, CancellationToken cancellationToken)
        {
            if (httpResponse.IsSuccessStatusCode)
            {
                return;
            }

            ProblemDetails? problemDetails = null;
            if (httpResponse.Content.Headers.ContentType?.MediaType == SupplementaryMediaTypeNames.Application.Problem.Json)
            {
                problemDetails = await httpResponse.Content.ReadFromJsonAsync<ProblemDetails>(jsonOptions, cancellationToken).WithoutCapturingContext();
            }

            throw new ApiException(httpResponse.GetStatusMessage())
            {
                StatusCode = httpResponse.StatusCode,
                ProblemDetails = problemDetails
            };
        }
    }
}
