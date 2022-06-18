using HR.BrightspaceConnector.Features.OrgUnits;
using HR.BrightspaceConnector.Features.Users;
using HR.BrightspaceConnector.Infrastructure;

namespace HR.BrightspaceConnector
{
    public interface IApiClient
    {
        #region Users
        /// <summary>
        /// Retrieve a list of all known user roles.
        /// </summary>
        /// <remarks>
        /// Oauth2 Scopes: <c>role:detail:read</c>
        /// </remarks>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IEnumerable<Role>> GetRolesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieve data for one or more users.
        /// </summary>
        /// <remarks>
        /// Input. You can use this action in a number of different ways, depending upon the query parameters you provide. 
        /// If you provide more than one of these parameters, this action selects among them in this order, regardless of the order you provide them in your URL:
        /// <list type="number">
        ///   <item>orgDefinedId. Find all users that have this org-defined ID string. Fetch the results in a JSON array.</item>
        ///   <item>userName. Find the single user that has this user name.</item>
        ///   <item>externalEmail. Find all users that exactly match this external email address string. Fetch the results in a JSON array.</item>
        ///   <item>bookmark. Use a paged result set to return the results. Fetch the segment of results immediately following your bookmark.</item>
        /// </list>
        /// If you provide none of these query parameters, this action behaves as if you'd passed an empty value for the bookmark parameter: it fetches the first segment of results.
        /// <para>
        /// Oauth2 Scopes: <c>users:userdata:read</c>
        /// </para>
        /// </remarks>
        /// <param name="queryParameters"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>A (possibly empty) <see cref="IEnumerable{UserData}"/> over the results. For the default and 'bookmark' cases, this IEnumerable will be of type <see cref="PagedResultSet{UserData}"/>.</returns>
        Task<IEnumerable<UserData>> GetUsersAsync(UserQueryParameters? queryParameters, CancellationToken cancellationToken = default);

        /// <summary>
        /// Create a new user entity.
        /// </summary>
        /// <remarks>
        /// Oauth2 Scopes: <c>users:userdata:create</c>
        /// </remarks>
        /// <param name="user"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>This action returns a UserData JSON block for the newly created user, to give you immediate access to the user's UserId property.</returns>
        Task<UserData> CreateUserAsync(CreateUserData user, CancellationToken cancellationToken = default);

        /// <summary>
        /// Update data for a particular user.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="user"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>This action returns a UserData JSON block for the user's updated data.</returns>
        Task<UserData> UpdateUserAsync(int userId, UpdateUserData user, CancellationToken cancellationToken = default);

        /// <summary>
        /// Delete a particular user.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task DeleteUserAsync(int userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieve legal, preferred, and sort names for a particular user.
        /// </summary>
        /// <remarks>
        /// Oauth2 Scopes: <c>users:userdata:read</c>
        /// </remarks>
        /// <param name="userId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<LegalPreferredNames> GetLegalPreferredNamesAsync(int userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Update legal, preferred, and sort name data for a particular user.
        /// </summary>
        /// <remarks>
        /// Oauth2 Scopes: <c>users:userdata:update</c>
        /// </remarks>
        /// <param name="userId"></param>
        /// <param name="userNames"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>This action returns a LegalPreferredNames JSON block for the user's updated name data.</returns>
        Task<LegalPreferredNames> UpdateLegalPreferredNamesAsync(int userId, LegalPreferredNames userNames, CancellationToken cancellationToken = default);
        #endregion

        #region OrgUnits
        /// <summary>
        /// Retrieve the organization properties information.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>This action returns a Organization JSON data block containing the identifier, name, and time zone of the organization.</returns>
        Task<Organization> GetOrganizationAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieve all the known and visible org unit types.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IEnumerable<OrgUnitType>> GetOrgUnitTypes(CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieve a list of child-units for a provided org unit.
        /// </summary>
        /// <remarks>
        /// Oauth2 Scopes: <c>organizations:organization:read</c>
        /// </remarks>
        /// <param name="parentOrgUnitId">The ID of the OrgUnit to retrieve the immediate children of.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>This action returns a paged result set containing the resulting org unit blocks for the segment following your bookmark parameter (or the first segment if that parameter is empty or missing).</returns>
        Task<PagedResultSet<OrgUnit>> GetChildOrgUnitsAsync(int parentOrgUnitId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieve a list of descendent-units for a provided org unit.
        /// </summary>
        /// <remarks>
        /// Oauth2 Scopes: <c>organizations:organization:read</c>
        /// </remarks>
        /// <param name="ancestorOrgUnitId">The ID of the OrgUnit to retrieve the descendants of.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>This action returns a paged result set containing the resulting org unit blocks for the segment following your bookmark parameter (or the first segment if that parameter is empty or missing).</returns>
        Task<PagedResultSet<OrgUnit>> GetDescendantOrgUnitsAsync(int ancestorOrgUnitId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Create a new custom org unit.
        /// </summary>
        /// <remarks>
        /// Oauth2 Scopes: <c>organizations:organization:create</c>
        /// </remarks>
        /// <param name="orgUnit"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>This action returns an OrgUnit JSON data block containing the properties for the newly created org unit.</returns>
        Task<OrgUnit> CreateOrgUnitAsync(OrgUnitCreateData orgUnit, CancellationToken cancellationToken = default);

        /// <summary>
        /// Update a custom org unit's properties.
        /// </summary>
        /// <remarks>
        /// Note that you can only update the Name, Code, and Path properties; the values of the other proprties in the OrgUnitProperties you provide are not used to update your specified org unit.
        /// <para>
        /// Oauth2 Scopes: <c>organizations:organization:update</c>
        /// </para>
        /// </remarks>
        /// <param name="orgUnitId"></param>
        /// <param name="orgUnit"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>This action returns an OrgUnitProperties JSON data block containing the properties for the updated org unit.</returns>
        Task<OrgUnitProperties> UpdateOrgUnitAsync(int orgUnitId, OrgUnitProperties orgUnit, CancellationToken cancellationToken = default);

        /// <summary>
        /// Send an org unit to the recycle bin, with an option to immediately flush it from there as well, permanently deleting it.
        /// </summary>
        /// <remarks>
        /// Using this action has the same effect as deleting the org unit via the OrgUnit Editor (and placing it in the recycling bin) in the Brightspace UI.
        /// <para>
        /// Oauth2 Scopes: <c>organizations:organization:create</c>
        /// </para>
        /// </remarks>
        /// <param name="orgUnitId"></param>
        /// <param name="permanently">Permanently delete the org unit?</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task DeleteOrgUnitAsync(int orgUnitId, bool permanently = false, CancellationToken cancellationToken = default);
        #endregion
    }
}
