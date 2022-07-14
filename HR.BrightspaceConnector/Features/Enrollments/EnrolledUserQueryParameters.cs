using HR.BrightspaceConnector.Infrastructure;

namespace HR.BrightspaceConnector.Features.Enrollments
{
    public class EnrolledUserQueryParameters : QueryParametersBase
    {
        /// <summary>
        /// Optional. Filter list to a specific user role.
        /// </summary>
        public int? RoleId { get; set; }

        /// <summary>
        /// Optional. Filter list to only active or inactive users.
        /// </summary>
        public bool? IsActive { get; set; }

        /// <summary>
        /// Optional. Bookmark to use for fetching next data set segment.
        /// </summary>
        public string? Bookmark { get; set; }
    }
}
