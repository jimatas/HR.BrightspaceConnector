using HR.BrightspaceConnector.Features.Common;

namespace HR.BrightspaceConnector.Features.Sections
{
    /// <summary>
    /// When you use actions to fetch the settings for all sections in a course offering, the service provides a block that looks like this.
    /// </summary>
    public class SectionSettingsData : SectionSettingsDataBase
    {
        /// <summary>
        /// A name for all of the sections in this course offering collectively.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// A description for all of the sections in this course offering collectively.
        /// </summary>
        public RichText? Description { get; set; }
        public SectionEnrollmentStyle? EnrollmentStyle { get; set; }
        public int? EnrollmentQuantity { get; set; }
    }
}
