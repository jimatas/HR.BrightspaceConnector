using HR.BrightspaceConnector.Features.Common;

namespace HR.BrightspaceConnector.Features.Sections
{
    /// <summary>
    /// When you use actions to fetch the properties of a section, the service provides a block that looks like this.
    /// </summary>
    public class SectionData : SectionDataBase
    {
        public int? SectionId { get; set; }
        public RichText? Description { get; set; }

        /// <summary>
        /// This property contains a list of user IDs for the explicitly enrolled users enrolled in the section.
        /// </summary>
        public IEnumerable<int> Enrollments { get; set; } = Enumerable.Empty<int>();
    }
}
