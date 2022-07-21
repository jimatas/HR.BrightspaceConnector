using HR.BrightspaceConnector.Features.Common;

namespace HR.BrightspaceConnector.Features.Sections
{
    public static class SectionRecordExtensions
    {
        public static CreateOrUpdateSectionData ToCreateOrUpdateSectionData(this SectionRecord sectionRecord)
        {
            return new CreateOrUpdateSectionData
            {
                Code = sectionRecord.Code,
                Description = !string.IsNullOrEmpty(sectionRecord.Description)
                    ? new RichTextInput { Content = sectionRecord.Description, Type = TextContentType.Html }
                    : null,
                Name = sectionRecord.Name
            };
        }
    }
}
