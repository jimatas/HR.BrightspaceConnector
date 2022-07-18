using HR.BrightspaceConnector.Features.Common;

namespace HR.BrightspaceConnector.Features.Sections
{
    /// <summary>
    /// When you use actions that update the settings for all sections in a course offering, you should provide a block that looks like this.
    /// </summary>
    public class UpdateSectionSettingsData : SectionSettingsDataBase
    {
        /// <summary>
        /// A name for all of the sections in this course offering collectively.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// A description for all of the sections in this course offering collectively.
        /// </summary>
        public RichTextInput? Description { get; set; }
    }
}
