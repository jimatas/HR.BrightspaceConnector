using HR.BrightspaceConnector.Features.Users;
using HR.BrightspaceConnector.Infrastructure;

namespace HR.BrightspaceConnector
{
    public interface IApiClient
    {
        /// <summary>
        /// Retrieve a list of all known user roles.
        /// </summary>
        /// <remarks>
        /// Requires the OAuth2 scope(s): <c>role:detail:read</c>
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
        /// Requires the OAuth2 scope(s): <c>users:userdata:read</c>
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
        /// Requires the OAuth2 scope(s): <c>users:userdata:create</c>
        /// </remarks>
        /// <param name="user"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>This action returns a UserData JSON block for the newly created user, to give you immediate access to the user's UserId property.</returns>
        Task<UserData> CreateUserAsync(CreateUserData user, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieve legal, preferred, and sort names for a particular user.
        /// </summary>
        /// <remarks>
        /// Requires the OAuth2 scope(s): <c>users:userdata:read</c>
        /// </remarks>
        /// <param name="userId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<LegalPreferredNames> GetLegalPreferredNamesAsync(int userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Update legal, preferred, and sort name data for a particular user.
        /// </summary>
        /// <remarks>
        /// Requires the OAuth2 scope(s): <c>users:userdata:update</c>
        /// </remarks>
        /// <param name="userId"></param>
        /// <param name="userNames"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>This action returns a LegalPreferredNames JSON block for the user's updated name data.</returns>
        Task<LegalPreferredNames> UpdateLegalPreferredNamesAsync(int userId, LegalPreferredNames userNames, CancellationToken cancellationToken = default);
    }
}
