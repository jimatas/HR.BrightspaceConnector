using HR.BrightspaceConnector.Features.Courses;
using HR.BrightspaceConnector.Features.Enrollments;
using HR.BrightspaceConnector.Features.OrgUnits;
using HR.BrightspaceConnector.Features.Sections;
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
            await SetAuthorizationHeaderAsync(httpRequest, cancellationToken).WithoutCapturingContext();

            using var httpResponse = await httpClient.SendAsync(httpRequest, cancellationToken).WithoutCapturingContext();
            await CheckResponseForErrorAsync(httpResponse, cancellationToken).WithoutCapturingContext();

            var roles = await httpResponse.Content.ReadFromJsonAsync<IEnumerable<Role>>(jsonOptions, cancellationToken).WithoutCapturingContext();
            return roles!;
        }

        public async Task<IEnumerable<UserData>> GetUsersAsync(UserQueryParameters? queryParameters = null, CancellationToken cancellationToken = default)
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"lp/{apiSettings.LearningPlatformVersion}/users/{queryParameters?.ToQueryString(jsonOptions.PropertyNamingPolicy)}");
            await SetAuthorizationHeaderAsync(httpRequest, cancellationToken).WithoutCapturingContext();

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
            await SetAuthorizationHeaderAsync(httpRequest, cancellationToken).WithoutCapturingContext();

            using var httpResponse = await httpClient.SendAsync(httpRequest, cancellationToken).WithoutCapturingContext();
            await CheckResponseForErrorAsync(httpResponse, cancellationToken).WithoutCapturingContext();

            var newUser = await httpResponse.Content.ReadFromJsonAsync<UserData>(jsonOptions, cancellationToken).WithoutCapturingContext();
            return newUser!;
        }

        public async Task<UserData> UpdateUserAsync(int userId, UpdateUserData user, CancellationToken cancellationToken = default)
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Put, $"lp/{apiSettings.LearningPlatformVersion}/users/{userId}") { Content = JsonContent.Create(user, mediaType: null, jsonOptions) };
            await SetAuthorizationHeaderAsync(httpRequest, cancellationToken).WithoutCapturingContext();

            using var httpResponse = await httpClient.SendAsync(httpRequest, cancellationToken).WithoutCapturingContext();
            await CheckResponseForErrorAsync(httpResponse, cancellationToken).WithoutCapturingContext();

            var updatedUser = await httpResponse.Content.ReadFromJsonAsync<UserData>(jsonOptions, cancellationToken).WithoutCapturingContext();
            return updatedUser!;
        }

        public async Task DeleteUserAsync(int userId, CancellationToken cancellationToken = default)
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Delete, $"lp/{apiSettings.LearningPlatformVersion}/users/{userId}");
            await SetAuthorizationHeaderAsync(httpRequest, cancellationToken).WithoutCapturingContext();

            using var httpResponse = await httpClient.SendAsync(httpRequest, cancellationToken).WithoutCapturingContext();
            await CheckResponseForErrorAsync(httpResponse, cancellationToken).WithoutCapturingContext();
        }

        public async Task<LegalPreferredNames> GetLegalPreferredNamesAsync(int userId, CancellationToken cancellationToken = default)
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"lp/{apiSettings.LearningPlatformVersion}/users/{userId}/names");
            await SetAuthorizationHeaderAsync(httpRequest, cancellationToken).WithoutCapturingContext();

            using var httpResponse = await httpClient.SendAsync(httpRequest, cancellationToken).WithoutCapturingContext();
            await CheckResponseForErrorAsync(httpResponse, cancellationToken).WithoutCapturingContext();

            var userNames = await httpResponse.Content.ReadFromJsonAsync<LegalPreferredNames>(jsonOptions, cancellationToken).WithoutCapturingContext();
            return userNames!;
        }

        public async Task<LegalPreferredNames> UpdateLegalPreferredNamesAsync(int userId, LegalPreferredNames userNames, CancellationToken cancellationToken = default)
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Put, $"lp/{apiSettings.LearningPlatformVersion}/users/{userId}/names") { Content = JsonContent.Create(userNames, mediaType: null, jsonOptions) };
            await SetAuthorizationHeaderAsync(httpRequest, cancellationToken).WithoutCapturingContext();

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

        public async Task<IEnumerable<OrgUnitType>> GetOrgUnitTypesAsync(CancellationToken cancellationToken = default)
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"lp/{apiSettings.LearningPlatformVersion}/outypes/");
            await SetAuthorizationHeaderAsync(httpRequest, cancellationToken).WithoutCapturingContext();

            using var httpResponse = await httpClient.SendAsync(httpRequest, cancellationToken).WithoutCapturingContext();
            await CheckResponseForErrorAsync(httpResponse, cancellationToken).WithoutCapturingContext();

            var orgUnitTypes = await httpResponse.Content.ReadFromJsonAsync<IEnumerable<OrgUnitType>>(jsonOptions, cancellationToken).WithoutCapturingContext();
            return orgUnitTypes!;
        }

        public async Task<PagedResultSet<OrgUnitProperties>> GetOrgUnitsAsync(OrgUnitQueryParameters? queryParameters = null, CancellationToken cancellationToken = default)
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"lp/{apiSettings.LearningPlatformVersion}/orgstructure/{queryParameters?.ToQueryString(jsonOptions.PropertyNamingPolicy)}");
            await SetAuthorizationHeaderAsync(httpRequest, cancellationToken).WithoutCapturingContext();

            using var httpResponse = await httpClient.SendAsync(httpRequest, cancellationToken).WithoutCapturingContext();

            var orgUnits = await httpResponse.Content.ReadFromJsonAsync<PagedResultSet<OrgUnitProperties>>(jsonOptions, cancellationToken).WithoutCapturingContext();
            return orgUnits!;
        }

        public async Task<PagedResultSet<OrgUnit>> GetChildOrgUnitsAsync(int parentOrgUnitId, CancellationToken cancellationToken = default)
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"lp/{apiSettings.LearningPlatformVersion}/orgstructure/{parentOrgUnitId}/children/paged/");
            await SetAuthorizationHeaderAsync(httpRequest, cancellationToken).WithoutCapturingContext();

            using var httpResponse = await httpClient.SendAsync(httpRequest, cancellationToken).WithoutCapturingContext();
            await CheckResponseForErrorAsync(httpResponse, cancellationToken).WithoutCapturingContext();

            var children = await httpResponse.Content.ReadFromJsonAsync<PagedResultSet<OrgUnit>>(jsonOptions, cancellationToken).WithoutCapturingContext();
            return children!;
        }

        public async Task<PagedResultSet<OrgUnit>> GetDescendantOrgUnitsAsync(int ancestorOrgUnitId, CancellationToken cancellationToken = default)
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"lp/{apiSettings.LearningPlatformVersion}/orgstructure/{ancestorOrgUnitId}/descendants/paged/");
            await SetAuthorizationHeaderAsync(httpRequest, cancellationToken).WithoutCapturingContext();

            using var httpResponse = await httpClient.SendAsync(httpRequest, cancellationToken).WithoutCapturingContext();
            await CheckResponseForErrorAsync(httpResponse, cancellationToken).WithoutCapturingContext();

            var descendants = await httpResponse.Content.ReadFromJsonAsync<PagedResultSet<OrgUnit>>(jsonOptions, cancellationToken).WithoutCapturingContext();
            return descendants!;
        }

        public async Task<OrgUnit> CreateOrgUnitAsync(OrgUnitCreateData orgUnit, CancellationToken cancellationToken = default)
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"lp/{apiSettings.LearningPlatformVersion}/orgstructure/") { Content = JsonContent.Create(orgUnit, mediaType: null, jsonOptions) };
            await SetAuthorizationHeaderAsync(httpRequest, cancellationToken).WithoutCapturingContext();

            using var httpResponse = await httpClient.SendAsync(httpRequest, cancellationToken).WithoutCapturingContext();
            await CheckResponseForErrorAsync(httpResponse, cancellationToken).WithoutCapturingContext();

            var newOrgUnit = await httpResponse.Content.ReadFromJsonAsync<OrgUnit>(jsonOptions, cancellationToken).WithoutCapturingContext();
            return newOrgUnit!;
        }

        public async Task<OrgUnitProperties> UpdateOrgUnitAsync(int orgUnitId, OrgUnitProperties orgUnit, CancellationToken cancellationToken = default)
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Put, $"lp/{apiSettings.LearningPlatformVersion}/orgstructure/{orgUnitId}") { Content = JsonContent.Create(orgUnit, mediaType: null, jsonOptions) };
            await SetAuthorizationHeaderAsync(httpRequest, cancellationToken).WithoutCapturingContext();

            using var httpResponse = await httpClient.SendAsync(httpRequest, cancellationToken).WithoutCapturingContext();
            await CheckResponseForErrorAsync(httpResponse, cancellationToken).WithoutCapturingContext();

            var updatedOrgUnit = await httpResponse.Content.ReadFromJsonAsync<OrgUnitProperties>(jsonOptions, cancellationToken).WithoutCapturingContext();
            return updatedOrgUnit!;
        }

        public async Task DeleteOrgUnitAsync(int orgUnitId, bool permanently = false, CancellationToken cancellationToken = default)
        {
            await MoveOrgUnitToRecycleBinAsync(orgUnitId, cancellationToken).WithoutCapturingContext();
            if (permanently)
            {
                await DeleteOrgUnitFromRecycleBinAsync(orgUnitId, cancellationToken).WithoutCapturingContext();
            }
        }

        public async Task MoveOrgUnitToRecycleBinAsync(int orgUnitId, CancellationToken cancellationToken = default)
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"lp/{apiSettings.LearningPlatformVersion}/orgstructure/recyclebin/{orgUnitId}/recycle");
            await SetAuthorizationHeaderAsync(httpRequest, cancellationToken).WithoutCapturingContext();

            using var httpResponse = await httpClient.SendAsync(httpRequest, cancellationToken).WithoutCapturingContext();
            await CheckResponseForErrorAsync(httpResponse, cancellationToken).WithoutCapturingContext();
        }

        public async Task DeleteOrgUnitFromRecycleBinAsync(int orgUnitId, CancellationToken cancellationToken = default)
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Delete, $"lp/{apiSettings.LearningPlatformVersion}/orgstructure/recyclebin/{orgUnitId}");
            await SetAuthorizationHeaderAsync(httpRequest, cancellationToken).WithoutCapturingContext();

            using var httpResponse = await httpClient.SendAsync(httpRequest, cancellationToken).WithoutCapturingContext();
            await CheckResponseForErrorAsync(httpResponse, cancellationToken).WithoutCapturingContext();
        }
        #endregion

        #region Courses
        public async Task<CourseTemplate> GetCourseTemplateAsync(int orgUnitId, CancellationToken cancellationToken = default)
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"lp/{apiSettings.LearningPlatformVersion}/coursetemplates/{orgUnitId}");
            await SetAuthorizationHeaderAsync(httpRequest, cancellationToken).WithoutCapturingContext();

            using var httpResponse = await httpClient.SendAsync(httpRequest, cancellationToken).WithoutCapturingContext();
            await CheckResponseForErrorAsync(httpResponse, cancellationToken).WithoutCapturingContext();

            var courseTemplate = await httpResponse.Content.ReadFromJsonAsync<CourseTemplate>(jsonOptions, cancellationToken).WithoutCapturingContext();
            return courseTemplate!;
        }

        public async Task<CourseTemplate> CreateCourseTemplateAsync(CreateCourseTemplate courseTemplate, CancellationToken cancellationToken = default)
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"lp/{apiSettings.LearningPlatformVersion}/coursetemplates/") { Content = JsonContent.Create(courseTemplate, mediaType: null, jsonOptions) };
            await SetAuthorizationHeaderAsync(httpRequest, cancellationToken).WithoutCapturingContext();

            using var httpResponse = await httpClient.SendAsync(httpRequest, cancellationToken).WithoutCapturingContext();
            await CheckResponseForErrorAsync(httpResponse, cancellationToken).WithoutCapturingContext();

            var newCourseTemplate = await httpResponse.Content.ReadFromJsonAsync<CourseTemplate>(jsonOptions, cancellationToken).WithoutCapturingContext();
            return newCourseTemplate!;
        }

        public async Task UpdateCourseTemplateAsync(int orgUnitId, CourseTemplateInfo courseTemplate, CancellationToken cancellationToken = default)
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Put, $"lp/{apiSettings.LearningPlatformVersion}/coursetemplates/{orgUnitId}") { Content = JsonContent.Create(courseTemplate, mediaType: null, jsonOptions) };
            await SetAuthorizationHeaderAsync(httpRequest, cancellationToken).WithoutCapturingContext();

            using var httpResponse = await httpClient.SendAsync(httpRequest, cancellationToken).WithoutCapturingContext();
            await CheckResponseForErrorAsync(httpResponse, cancellationToken).WithoutCapturingContext();
        }

        public async Task DeleteCourseTemplateAsync(int orgUnitId, bool permanently = false, CancellationToken cancellationToken = default)
        {
            await DeleteOrgUnitAsync(orgUnitId, permanently, cancellationToken).WithoutCapturingContext();
        }

        public async Task<CourseOffering> GetCourseOfferingAsync(int orgUnitId, CancellationToken cancellationToken = default)
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"lp/{apiSettings.LearningPlatformVersion}/courses/{orgUnitId}");
            await SetAuthorizationHeaderAsync(httpRequest, cancellationToken).WithoutCapturingContext();

            using var httpResponse = await httpClient.SendAsync(httpRequest, cancellationToken).WithoutCapturingContext();
            await CheckResponseForErrorAsync(httpResponse, cancellationToken).WithoutCapturingContext();

            var courseOffering = await httpResponse.Content.ReadFromJsonAsync<CourseOffering>(jsonOptions, cancellationToken).WithoutCapturingContext();
            return courseOffering!;
        }

        public async Task<CourseOffering> CreateCourseOfferingAsync(CreateCourseOffering courseOffering, CancellationToken cancellationToken = default)
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"lp/{apiSettings.LearningPlatformVersion}/courses/") { Content = JsonContent.Create(courseOffering, mediaType: null, jsonOptions) };
            await SetAuthorizationHeaderAsync(httpRequest, cancellationToken).WithoutCapturingContext();

            using var httpResponse = await httpClient.SendAsync(httpRequest, cancellationToken).WithoutCapturingContext();
            await CheckResponseForErrorAsync(httpResponse, cancellationToken).WithoutCapturingContext();

            var newCourseOffering = await httpResponse.Content.ReadFromJsonAsync<CourseOffering>(jsonOptions, cancellationToken).WithoutCapturingContext();
            return newCourseOffering!;
        }

        public async Task UpdateCourseOfferingAsync(int orgUnitId, CourseOfferingInfo courseOffering, CancellationToken cancellationToken = default)
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Put, $"lp/{apiSettings.LearningPlatformVersion}/courses/{orgUnitId}") { Content = JsonContent.Create(courseOffering, mediaType: null, jsonOptions) };
            await SetAuthorizationHeaderAsync(httpRequest, cancellationToken).WithoutCapturingContext();

            using var httpResponse = await httpClient.SendAsync(httpRequest, cancellationToken).WithoutCapturingContext();
            await CheckResponseForErrorAsync(httpResponse, cancellationToken).WithoutCapturingContext();
        }

        public async Task DeleteCourseOfferingAsync(int orgUnitId, bool permanently = false, CancellationToken cancellationToken = default)
        {
            await DeleteOrgUnitAsync(orgUnitId, permanently, cancellationToken).WithoutCapturingContext();
        }
        #endregion

        #region Enrollments
        public async Task<EnrollmentData> GetEnrollmentAsync(int userId, int orgUnitId, CancellationToken cancellationToken = default)
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"lp/{apiSettings.LearningPlatformVersion}/enrollments/users/{userId}/orgUnits/{orgUnitId}");
            await SetAuthorizationHeaderAsync(httpRequest, cancellationToken).WithoutCapturingContext();

            using var httpResponse = await httpClient.SendAsync(httpRequest, cancellationToken).WithoutCapturingContext();
            await CheckResponseForErrorAsync(httpResponse, cancellationToken).WithoutCapturingContext();

            var enrollment = await httpResponse.Content.ReadFromJsonAsync<EnrollmentData>(jsonOptions, cancellationToken).WithoutCapturingContext();
            return enrollment!;
        }

        public async Task<PagedResultSet<OrgUnitUser>> GetEnrolledUsersAsync(int orgUnitId, EnrolledUserQueryParameters? queryParameters = null, CancellationToken cancellationToken = default)
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"lp/{apiSettings.LearningPlatformVersion}/enrollments/orgUnits/{orgUnitId}/users/{queryParameters?.ToQueryString(jsonOptions.PropertyNamingPolicy)}");
            await SetAuthorizationHeaderAsync(httpRequest, cancellationToken).WithoutCapturingContext();

            using var httpResponse = await httpClient.SendAsync(httpRequest, cancellationToken).WithoutCapturingContext();
            await CheckResponseForErrorAsync(httpResponse, cancellationToken).WithoutCapturingContext();

            var classlistUsers = await httpResponse.Content.ReadFromJsonAsync<PagedResultSet<OrgUnitUser>>(jsonOptions, cancellationToken).WithoutCapturingContext();
            return classlistUsers!;
        }

        public async Task<EnrollmentData> CreateOrUpdateEnrollmentAsync(CreateEnrollmentData enrollment, CancellationToken cancellationToken = default)
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"lp/{apiSettings.LearningPlatformVersion}/enrollments/") { Content = JsonContent.Create(enrollment, mediaType: null, jsonOptions) };
            await SetAuthorizationHeaderAsync(httpRequest, cancellationToken).WithoutCapturingContext();

            using var httpResponse = await httpClient.SendAsync(httpRequest, cancellationToken).WithoutCapturingContext();
            await CheckResponseForErrorAsync(httpResponse, cancellationToken).WithoutCapturingContext();

            var newEnrollment = await httpResponse.Content.ReadFromJsonAsync<EnrollmentData>(jsonOptions, cancellationToken).WithoutCapturingContext();
            return newEnrollment!;
        }

        public async Task<EnrollmentData> DeleteEnrollmentAsync(int userId, int orgUnitId, CancellationToken cancellationToken = default)
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Delete, $"lp/{apiSettings.LearningPlatformVersion}/enrollments/users/{userId}/orgUnits/{orgUnitId}");
            await SetAuthorizationHeaderAsync(httpRequest, cancellationToken).WithoutCapturingContext();

            using var httpResponse = await httpClient.SendAsync(httpRequest, cancellationToken).WithoutCapturingContext();
            await CheckResponseForErrorAsync(httpResponse, cancellationToken).WithoutCapturingContext();

            var deletedEnrollment = await httpResponse.Content.ReadFromJsonAsync<EnrollmentData>(jsonOptions, cancellationToken).WithoutCapturingContext();
            return deletedEnrollment!;
        }
        #endregion

        #region Sections
        public async Task<SectionSettingsData> GetSectionSettingsAsync(int orgUnitId, CancellationToken cancellationToken = default)
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"lp/{apiSettings.LearningPlatformVersion}/{orgUnitId}/sections/settings");
            await SetAuthorizationHeaderAsync(httpRequest, cancellationToken).WithoutCapturingContext();

            using var httpResponse = await httpClient.SendAsync(httpRequest, cancellationToken).WithoutCapturingContext();
            await CheckResponseForErrorAsync(httpResponse, cancellationToken).WithoutCapturingContext();

            var sectionSettings = await httpResponse.Content.ReadFromJsonAsync<SectionSettingsData>(jsonOptions, cancellationToken).WithoutCapturingContext();
            return sectionSettings!;
        }

        public async Task<SectionSettingsData> CreateSectionSettingsAsync(int orgUnitId, CreateSectionSettingsData sectionSettings, CancellationToken cancellationToken = default)
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"lp/{apiSettings.LearningPlatformVersion}/{orgUnitId}/sections/settings") { Content = JsonContent.Create(sectionSettings, mediaType: null, jsonOptions) };
            await SetAuthorizationHeaderAsync(httpRequest, cancellationToken).WithoutCapturingContext();

            using var httpResponse = await httpClient.SendAsync(httpRequest, cancellationToken).WithoutCapturingContext();
            await CheckResponseForErrorAsync(httpResponse, cancellationToken).WithoutCapturingContext();

            var newSectionSettings = await httpResponse.Content.ReadFromJsonAsync<SectionSettingsData>(jsonOptions, cancellationToken).WithoutCapturingContext();
            return newSectionSettings!;
        }

        public async Task<SectionSettingsData> UpdateSectionSettingsAsync(int orgUnitId, UpdateSectionSettingsData sectionSettings, CancellationToken cancellationToken = default)
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Put, $"lp/{apiSettings.LearningPlatformVersion}/{orgUnitId}/sections/settings") { Content = JsonContent.Create(sectionSettings, mediaType: null, jsonOptions) };
            await SetAuthorizationHeaderAsync(httpRequest, cancellationToken).WithoutCapturingContext();

            using var httpResponse = await httpClient.SendAsync(httpRequest, cancellationToken).WithoutCapturingContext();
            await CheckResponseForErrorAsync(httpResponse, cancellationToken).WithoutCapturingContext();

            var newSectionSettings = await httpResponse.Content.ReadFromJsonAsync<SectionSettingsData>(jsonOptions, cancellationToken).WithoutCapturingContext();
            return newSectionSettings!;
        }

        public async Task<IEnumerable<SectionData>> GetSectionsAsync(int orgUnitId, CancellationToken cancellationToken = default)
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"lp/{apiSettings.LearningPlatformVersion}/{orgUnitId}/sections/");
            await SetAuthorizationHeaderAsync(httpRequest, cancellationToken).WithoutCapturingContext();

            using var httpResponse = await httpClient.SendAsync(httpRequest, cancellationToken).WithoutCapturingContext();
            await CheckResponseForErrorAsync(httpResponse, cancellationToken).WithoutCapturingContext();

            var sections = await httpResponse.Content.ReadFromJsonAsync<IEnumerable<SectionData>>(jsonOptions, cancellationToken).WithoutCapturingContext();
            return sections!;
        }

        public async Task<SectionData> CreateSectionAsync(int orgUnitId, CreateOrUpdateSectionData section, CancellationToken cancellationToken = default)
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"lp/{apiSettings.LearningPlatformVersion}/{orgUnitId}/sections/") { Content = JsonContent.Create(section, mediaType: null, jsonOptions) };
            await SetAuthorizationHeaderAsync(httpRequest, cancellationToken).WithoutCapturingContext();

            using var httpResponse = await httpClient.SendAsync(httpRequest, cancellationToken).WithoutCapturingContext();
            await CheckResponseForErrorAsync(httpResponse, cancellationToken).WithoutCapturingContext();

            var newSection = await httpResponse.Content.ReadFromJsonAsync<SectionData>(jsonOptions, cancellationToken).WithoutCapturingContext();
            return newSection!;
        }

        public async Task CreateSectionEnrollmentAsync(int orgUnitId, int sectionId, SectionEnrollment enrollment, CancellationToken cancellationToken = default)
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"lp/{apiSettings.LearningPlatformVersion}/{orgUnitId}/sections/{sectionId}/enrollments/") { Content = JsonContent.Create(enrollment, mediaType: null, jsonOptions) };
            await SetAuthorizationHeaderAsync(httpRequest, cancellationToken).WithoutCapturingContext();

            using var httpResponse = await httpClient.SendAsync(httpRequest, cancellationToken).WithoutCapturingContext();
            await CheckResponseForErrorAsync(httpResponse, cancellationToken).WithoutCapturingContext();
        }

        public async Task<SectionData> UpdateSectionAsync(int orgUnitId, int sectionId, CreateOrUpdateSectionData section, CancellationToken cancellationToken = default)
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Put, $"lp/{apiSettings.LearningPlatformVersion}/{orgUnitId}/sections/{sectionId}") { Content = JsonContent.Create(section, mediaType: null, jsonOptions) };
            await SetAuthorizationHeaderAsync(httpRequest, cancellationToken).WithoutCapturingContext();

            using var httpResponse = await httpClient.SendAsync(httpRequest, cancellationToken).WithoutCapturingContext();
            await CheckResponseForErrorAsync(httpResponse, cancellationToken).WithoutCapturingContext();

            var updatedSection = await httpResponse.Content.ReadFromJsonAsync<SectionData>(jsonOptions, cancellationToken).WithoutCapturingContext();
            return updatedSection!;
        }

        public async Task DeleteSectionAsync(int orgUnitId, int sectionId, CancellationToken cancellationToken = default)
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Delete, $"lp/{apiSettings.LearningPlatformVersion}/{orgUnitId}/sections/{sectionId}");
            await SetAuthorizationHeaderAsync(httpRequest, cancellationToken).WithoutCapturingContext();

            using var httpResponse = await httpClient.SendAsync(httpRequest, cancellationToken).WithoutCapturingContext();
            await CheckResponseForErrorAsync(httpResponse, cancellationToken).WithoutCapturingContext();
        }
        #endregion

        private async Task SetAuthorizationHeaderAsync(HttpRequestMessage httpRequest, CancellationToken cancellationToken)
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
