using HR.BrightspaceConnector.Features.Roles;

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
    }
}
