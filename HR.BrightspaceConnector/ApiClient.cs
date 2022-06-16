using HR.BrightspaceConnector.Features.OrgUnits;
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

        #region Users
        public async Task<IEnumerable<Role>> GetRolesAsync(CancellationToken cancellationToken = default)
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"lp/{apiSettings.LearningPlatformVersion}/roles/");
            await SetAuthorizationHeader(httpRequest, cancellationToken).WithoutCapturingContext();

            using var httpResponse = await httpClient.SendAsync(httpRequest, cancellationToken).WithoutCapturingContext();
            await CheckResponseForErrorAsync(httpResponse, cancellationToken).WithoutCapturingContext();

            var roles = await httpResponse.Content.ReadFromJsonAsync<IEnumerable<Role>>(jsonOptions, cancellationToken).WithoutCapturingContext();
            return roles!;
        }

        public async Task<IEnumerable<UserData>> GetUsersAsync(UserQueryParameters? queryParameters, CancellationToken cancellationToken = default)
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"lp/{apiSettings.LearningPlatformVersion}/users/{queryParameters?.ToQueryString(jsonOptions.PropertyNamingPolicy)}");
            await SetAuthorizationHeader(httpRequest, cancellationToken).WithoutCapturingContext();

            using var httpResponse = await httpClient.SendAsync(httpRequest, cancellationToken).WithoutCapturingContext();

            var users = await TryParseUserResponseAsync(queryParameters, httpResponse, cancellationToken).WithoutCapturingContext();
            return users!;
        }

        private async Task<IEnumerable<UserData>> TryParseUserResponseAsync(UserQueryParameters? queryParameters, HttpResponseMessage httpResponse, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(queryParameters?.UserName) ||
                !string.IsNullOrEmpty(queryParameters?.OrgDefinedId) ||
                !string.IsNullOrEmpty(queryParameters?.ExternalEmail))
            {
                if (httpResponse.StatusCode == HttpStatusCode.NotFound)
                {
                    return Enumerable.Empty<UserData>();
                }

                if (httpResponse.IsSuccessStatusCode)
                {
                    if (!string.IsNullOrEmpty(queryParameters.UserName))
                    {
                        var user = await httpResponse.Content.ReadFromJsonAsync<UserData>(jsonOptions, cancellationToken).WithoutCapturingContext();
                        return new[] { user! };
                    }

                    var users = await httpResponse.Content.ReadFromJsonAsync<IEnumerable<UserData>>(jsonOptions, cancellationToken).WithoutCapturingContext();
                    return users!;
                }
                // else, error status received; fall through.
            }

            await CheckResponseForErrorAsync(httpResponse, cancellationToken).WithoutCapturingContext();

            var pagedUsers = await httpResponse.Content.ReadFromJsonAsync<PagedResultSet<UserData>>(jsonOptions, cancellationToken).WithoutCapturingContext();
            return pagedUsers!;
        }

        public async Task<UserData> CreateUserAsync(CreateUserData user, CancellationToken cancellationToken = default)
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"lp/{apiSettings.LearningPlatformVersion}/users/") { Content = JsonContent.Create(user, mediaType: null, jsonOptions) };
            await SetAuthorizationHeader(httpRequest, cancellationToken).WithoutCapturingContext();

            using var httpResponse = await httpClient.SendAsync(httpRequest, cancellationToken).WithoutCapturingContext();
            await CheckResponseForErrorAsync(httpResponse, cancellationToken).WithoutCapturingContext();

            var newUser = await httpResponse.Content.ReadFromJsonAsync<UserData>(jsonOptions, cancellationToken).WithoutCapturingContext();
            return newUser!;
        }

        public async Task<UserData> UpdateUserAsync(int userId, UpdateUserData user, CancellationToken cancellationToken = default)
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Put, $"lp/{apiSettings.LearningPlatformVersion}/users/{userId}") { Content = JsonContent.Create(user, mediaType: null, jsonOptions) };
            await SetAuthorizationHeader(httpRequest, cancellationToken).WithoutCapturingContext();

            using var httpResponse = await httpClient.SendAsync(httpRequest, cancellationToken).WithoutCapturingContext();
            await CheckResponseForErrorAsync(httpResponse, cancellationToken).WithoutCapturingContext();

            var updatedUser = await httpResponse.Content.ReadFromJsonAsync<UserData>(jsonOptions, cancellationToken).WithoutCapturingContext();
            return updatedUser!;
        }

        public async Task DeleteUserAsync(int userId, CancellationToken cancellationToken = default)
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Delete, $"lp/{apiSettings.LearningPlatformVersion}/users/{userId}");
            await SetAuthorizationHeader(httpRequest, cancellationToken).WithoutCapturingContext();

            using var httpResponse = await httpClient.SendAsync(httpRequest, cancellationToken).WithoutCapturingContext();
            await CheckResponseForErrorAsync(httpResponse, cancellationToken).WithoutCapturingContext();
        }

        public async Task<LegalPreferredNames> GetLegalPreferredNamesAsync(int userId, CancellationToken cancellationToken = default)
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"lp/{apiSettings.LearningPlatformVersion}/users/{userId}/names");
            await SetAuthorizationHeader(httpRequest, cancellationToken).WithoutCapturingContext();

            using var httpResponse = await httpClient.SendAsync(httpRequest, cancellationToken).WithoutCapturingContext();
            await CheckResponseForErrorAsync(httpResponse, cancellationToken).WithoutCapturingContext();

            var userNames = await httpResponse.Content.ReadFromJsonAsync<LegalPreferredNames>(jsonOptions, cancellationToken).WithoutCapturingContext();
            return userNames!;
        }

        public async Task<LegalPreferredNames> UpdateLegalPreferredNamesAsync(int userId, LegalPreferredNames userNames, CancellationToken cancellationToken = default)
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Put, $"lp/{apiSettings.LearningPlatformVersion}/users/{userId}/names") { Content = JsonContent.Create(userNames, mediaType: null, jsonOptions) };
            await SetAuthorizationHeader(httpRequest, cancellationToken).WithoutCapturingContext();

            using var httpResponse = await httpClient.SendAsync(httpRequest, cancellationToken).WithoutCapturingContext();
            await CheckResponseForErrorAsync(httpResponse, cancellationToken).WithoutCapturingContext();

            var updatedUserNames = await httpResponse.Content.ReadFromJsonAsync<LegalPreferredNames>(jsonOptions, cancellationToken).WithoutCapturingContext();
            return updatedUserNames!;
        }
        #endregion

        #region OrgUnits
        public async Task<Organization> GetOrganizationAsync(CancellationToken cancellationToken = default)
        {
            var httpResponse = await httpClient.GetAsync($"lp/{apiSettings.LearningPlatformVersion}/organization/info", cancellationToken).WithoutCapturingContext();
            await CheckResponseForErrorAsync(httpResponse, cancellationToken).WithoutCapturingContext();

            var organization = await httpResponse.Content.ReadFromJsonAsync<Organization>(jsonOptions, cancellationToken).WithoutCapturingContext();
            return organization!;
        }

        public async Task<IEnumerable<OrgUnitType>> GetOrgUnitTypes(CancellationToken cancellationToken = default)
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"lp/{apiSettings.LearningPlatformVersion}/outypes/");
            await SetAuthorizationHeader(httpRequest, cancellationToken).WithoutCapturingContext();

            using var httpResponse = await httpClient.SendAsync(httpRequest, cancellationToken).WithoutCapturingContext();
            await CheckResponseForErrorAsync(httpResponse, cancellationToken).WithoutCapturingContext();

            var orgUnitTypes = await httpResponse.Content.ReadFromJsonAsync<IEnumerable<OrgUnitType>>(jsonOptions, cancellationToken).WithoutCapturingContext();
            return orgUnitTypes!;
        }

        public async Task<PagedResultSet<OrgUnit>> GetDescendantOrgUnitsAsync(int orgUnitId, CancellationToken cancellationToken = default)
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"lp/{apiSettings.LearningPlatformVersion}/orgstructure/{orgUnitId}/descendants/paged/");
            await SetAuthorizationHeader(httpRequest, cancellationToken).WithoutCapturingContext();

            using var httpResponse = await httpClient.SendAsync(httpRequest, cancellationToken).WithoutCapturingContext();
            await CheckResponseForErrorAsync(httpResponse, cancellationToken).WithoutCapturingContext();

            var descendants = await httpResponse.Content.ReadFromJsonAsync<PagedResultSet<OrgUnit>>(jsonOptions, cancellationToken).WithoutCapturingContext();
            return descendants!;
        }

        public async Task<OrgUnit> CreateOrgUnitAsync(OrgUnitCreateData orgUnit, CancellationToken cancellationToken = default)
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"lp/{apiSettings.LearningPlatformVersion}/orgstructure/") { Content = JsonContent.Create(orgUnit, mediaType: null, jsonOptions) };
            await SetAuthorizationHeader(httpRequest, cancellationToken).WithoutCapturingContext();

            using var httpResponse = await httpClient.SendAsync(httpRequest, cancellationToken).WithoutCapturingContext();
            await CheckResponseForErrorAsync(httpResponse, cancellationToken).WithoutCapturingContext();

            var newOrgUnit = await httpResponse.Content.ReadFromJsonAsync<OrgUnit>(jsonOptions, cancellationToken).WithoutCapturingContext();
            return newOrgUnit!;
        }
        #endregion

        private async Task SetAuthorizationHeader(HttpRequestMessage httpRequest, CancellationToken cancellationToken)
        {
            var oAuthToken = await tokenManager.GetTokenAsync(cancellationToken).WithoutCapturingContext();
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue(oAuthToken.TokenType ?? TokenResponse.DefaultTokenType, oAuthToken.AccessToken);
        }

        private async Task CheckResponseForErrorAsync(HttpResponseMessage httpResponse, CancellationToken cancellationToken)
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

            if (!(await httpResponse.TryGetErrorMessageAsync(jsonOptions, cancellationToken).WithoutCapturingContext()).Out(out var errorMessage))
            {
                errorMessage = httpResponse.ReasonPhrase;
            }

            throw new ApiException(errorMessage)
            {
                StatusCode = httpResponse.StatusCode,
                ProblemDetails = problemDetails
            };
        }
    }
}
