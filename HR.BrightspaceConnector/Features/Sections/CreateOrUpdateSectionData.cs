using HR.BrightspaceConnector.Features.Common;

namespace HR.BrightspaceConnector.Features.Sections
{
    /// <summary>
    /// When you use actions that provide section property information to the service, you should provide a block that looks like this.
    /// </summary>
    public class CreateOrUpdateSectionData : SectionDataBase
    {
        public RichTextInput? Description { get; set; }
    }
}
