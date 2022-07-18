namespace HR.BrightspaceConnector.Features.Sections
{
    /// <summary>
    /// When you use actions to initialize the settings for all sections in a course offering, you should provide a block that looks like this.
    /// </summary>
    public class CreateSectionSettingsData : SectionSettingsDataBase
    {
        public SectionEnrollmentStyle? EnrollmentStyle { get; set; }
        public int? EnrollmentQuantity { get; set; }
    }
}
