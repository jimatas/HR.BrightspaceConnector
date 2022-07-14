using HR.BrightspaceConnector.Features.Common;

namespace HR.BrightspaceConnector.Features.Courses
{
    /// <summary>
    /// The service's fundamental information block for course offerings. 
    /// Notice that some fields in this block include BasicOrgUnit blocks for the related org units.
    /// </summary>
    public class CourseOffering : CourseOfferingBase
    {
        public int? Identifier { get; set; }
        public bool? IsActive { get; set; }
        public string? Path { get; set; }
        public BasicOrgUnit? CourseTemplate { get; set; }
        public BasicOrgUnit? Semester { get; set; }
        public BasicOrgUnit? Department { get; set; }

        /// <summary>
        /// Added with LP API v1.26
        /// </summary>
        public RichText? Description { get; set; }
    }
}
