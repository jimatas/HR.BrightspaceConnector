using HR.BrightspaceConnector.Features.Users;
using HR.BrightspaceConnector.Infrastructure;

namespace HR.BrightspaceConnector
{
    public interface IApiClient
    {
        /// <summary>
        /// Retrieve a list of all known user roles.
        /// </summary>
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
        /// </remarks>
        /// <param name="queryParameters"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>A (possibly empty) <see cref="IEnumerable{UserData}"/> over the results. For the default and 'bookmark' cases, this IEnumerable will be of type <see cref="PagedResultSet{UserData}"/>.</returns>
        Task<IEnumerable<UserData>> GetUsersAsync(UserQueryParameters? queryParameters, CancellationToken cancellationToken = default);
    }
}
