using HR.BrightspaceConnector.Infrastructure;

namespace HR.BrightspaceConnector.Features.Enrollments
{
    public class ClasslistQueryParameters : QueryParametersBase
    {
        /// <summary>
        /// (Optional) If true, retrieve only gradeable users; false by default.
        /// </summary>
        public bool? OnlyShowShownInGrades { get; set; }

        /// <summary>
        /// (Optional) Search term to look for.
        /// </summary>
        /// <remarks>
        /// You can provide a searchTerm query parameter string to filter results returned; 
        /// the back-end service looks for fields that contain your search term in these ClasslistUser fields: UserName, OrgDefinedId, FirstName, LastName.
        /// </remarks>
        public string? SearchTerm { get; set; }
    }
}
