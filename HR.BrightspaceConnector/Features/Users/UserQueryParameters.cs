using HR.BrightspaceConnector.Infrastructure;

namespace HR.BrightspaceConnector.Features.Users
{
    public class UserQueryParameters : QueryParametersBase
    {
        /// <summary>
        /// Optional. Org-defined identifier to look for.
        /// </summary>
        public string? OrgDefinedId { get; set; }

        /// <summary>
        /// Optional. User name to look for.
        /// </summary>
        public string? UserName { get; set; }

        /// <summary>
        /// Optional. External email address to look for.
        /// </summary>
        public string? ExternalEmail { get; set; }

        /// <summary>
        /// Optional. Bookmark to use for fetching next data set segment.
        /// </summary>
        public string? Bookmark { get; set; }
    }
}
