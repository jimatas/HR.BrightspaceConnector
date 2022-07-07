namespace HR.BrightspaceConnector.Features.Courses
{
    /// <summary>
    /// Block used to create a course.
    /// </summary>
    public class CreateCourseOffering : CourseOfferingInfo
    {
        /// <summary>
        /// The root path to use for this course offering's course content. 
        /// Note that, if your back-end service has path enforcement set on for new org units, then you should leave this property as an empty string, and the back-end service can populate it for you.
        /// </summary>
        public string? Path { get; set; }

        public int? CourseTemplateId { get; set; }

        /// <summary>
        /// Note that if CreateCourse form does not include the Semester element, then you should provide null for this property when creating a course.
        /// </summary>
        public int? SemesterId { get; set; }

        public int? LocaleId { get; set; }

        /// <summary>
        /// Determines if the course should override the user's saved locale preference.
        /// </summary>
        public bool? ForceLocale { get; set; }

        /// <summary>
        /// Determines if the email tool's address book groups together the users enrolled in the course together.
        /// </summary>
        public bool? ShowAddressBook { get; set; }
    }
}
