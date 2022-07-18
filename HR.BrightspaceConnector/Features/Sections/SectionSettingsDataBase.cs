namespace HR.BrightspaceConnector.Features.Sections
{
    /// <summary>
    /// Serves as a base class for the <see cref="CreateSectionSettingsData"/>, <see cref="UpdateSectionSettingsData"/> and <see cref="SectionSettingsData"/> classes.
    /// </summary>
    public abstract class SectionSettingsDataBase
    {
        public bool? AutoEnroll { get; set; }
        public bool? RandomizeEnrollments { get; set; }

        /// <summary>
        /// Note that when this property is false, the Descriptions property in the Enrolled form of Section.SectionData will always be null for sections in this course offering.
        /// Added as of LP API version 1.39
        /// </summary>
        public bool? DescriptionsVisibleToEnrollees { get; set; }
    }
}
