﻿using HR.BrightspaceConnector.Features.OrgUnits;
using HR.BrightspaceConnector.Features.Users;
using HR.Common.Utilities;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace HR.BrightspaceConnector.Infrastructure
{
    public class CachingApiClient : IApiClient
    {
        private readonly ApiClient apiClient;
        private readonly ApiClientSettings apiSettings;
        private readonly IMemoryCache memoryCache;

        public CachingApiClient(ApiClient apiClient, IOptions<ApiClientSettings> apiSettings, IMemoryCache memoryCache)
        {
            this.apiClient = apiClient;
            this.apiSettings = apiSettings.Value;
            this.memoryCache = memoryCache;
        }

        #region Users
        public async Task<IEnumerable<Role>> GetRolesAsync(CancellationToken cancellationToken = default)
        {
            return await memoryCache.GetOrCreateAsync(
                GenerateCacheKey(nameof(GetRolesAsync)),
                async cacheEntry =>
                {
                    cacheEntry.AbsoluteExpirationRelativeToNow = CacheDuration;
                    return await apiClient.GetRolesAsync(cancellationToken).WithoutCapturingContext();
                }).WithoutCapturingContext();
        }

        public Task<IEnumerable<UserData>> GetUsersAsync(UserQueryParameters? queryParameters, CancellationToken cancellationToken = default)
        {
            return apiClient.GetUsersAsync(queryParameters, cancellationToken);
        }

        public Task<UserData> CreateUserAsync(CreateUserData user, CancellationToken cancellationToken = default)
        {
            return apiClient.CreateUserAsync(user, cancellationToken);
        }

        public Task<UserData> UpdateUserAsync(int userId, UpdateUserData user, CancellationToken cancellationToken = default)
        {
            return apiClient.UpdateUserAsync(userId, user, cancellationToken);
        }

        public Task DeleteUserAsync(int userId, CancellationToken cancellationToken = default)
        {
            return apiClient.DeleteUserAsync(userId, cancellationToken);
        }

        public Task<LegalPreferredNames> GetLegalPreferredNamesAsync(int userId, CancellationToken cancellationToken = default)
        {
            return apiClient.GetLegalPreferredNamesAsync(userId, cancellationToken);
        }

        public Task DeleteOrgUnitAsync(int orgUnitId, bool permanently = false, CancellationToken cancellationToken = default)
        {
            return apiClient.DeleteOrgUnitAsync(orgUnitId, permanently, cancellationToken);
        }

        public Task<LegalPreferredNames> UpdateLegalPreferredNamesAsync(int userId, LegalPreferredNames userNames, CancellationToken cancellationToken = default)
        {
            return apiClient.UpdateLegalPreferredNamesAsync(userId, userNames, cancellationToken);
        }
        #endregion

        #region OrgUnits
        public async Task<Organization> GetOrganizationAsync(CancellationToken cancellationToken = default)
        {
            return await memoryCache.GetOrCreateAsync(
                GenerateCacheKey(nameof(GetOrganizationAsync)),
                async cacheEntry =>
                {
                    cacheEntry.AbsoluteExpirationRelativeToNow = CacheDuration;
                    return await apiClient.GetOrganizationAsync(cancellationToken).WithoutCapturingContext();
                }).WithoutCapturingContext();
        }

        public async Task<IEnumerable<OrgUnitType>> GetOrgUnitTypes(CancellationToken cancellationToken = default)
        {
            return await memoryCache.GetOrCreateAsync(
                GenerateCacheKey(nameof(GetOrgUnitTypes)),
                async cacheEntry =>
                {
                    cacheEntry.AbsoluteExpirationRelativeToNow = CacheDuration;
                    return await apiClient.GetOrgUnitTypes(cancellationToken).WithoutCapturingContext();
                }).WithoutCapturingContext();
        }

        public Task<PagedResultSet<OrgUnit>> GetChildOrgUnitsAsync(int parentOrgUnitId, CancellationToken cancellationToken = default)
        {
            return apiClient.GetChildOrgUnitsAsync(parentOrgUnitId, cancellationToken);
        }

        public Task<PagedResultSet<OrgUnit>> GetDescendantOrgUnitsAsync(int ancestorOrgUnitId, CancellationToken cancellationToken = default)
        {
            return apiClient.GetDescendantOrgUnitsAsync(ancestorOrgUnitId, cancellationToken);
        }

        public Task<OrgUnit> CreateOrgUnitAsync(OrgUnitCreateData orgUnit, CancellationToken cancellationToken = default)
        {
            return apiClient.CreateOrgUnitAsync(orgUnit, cancellationToken);
        }

        public Task<OrgUnitProperties> UpdateOrgUnitAsync(int orgUnitId, OrgUnitProperties orgUnit, CancellationToken cancellationToken = default)
        {
            return apiClient.UpdateOrgUnitAsync(orgUnitId, orgUnit, cancellationToken);
        }
        #endregion

        private TimeSpan CacheDuration => apiSettings.CacheDuration ?? TimeSpan.Zero;
        private string GenerateCacheKey(string methodName) => $"{nameof(CachingApiClient)}[{apiSettings.BaseAddress}].{methodName}";
    }
}
