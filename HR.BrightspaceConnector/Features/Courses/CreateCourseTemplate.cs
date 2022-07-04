namespace HR.BrightspaceConnector.Features.Courses
{
    public class CreateCourseTemplate : CourseTemplateInfo
    {
        /// <summary>
        /// The root path to use for this course template's course content. 
        /// Note that, if your back-end service has path enforcement set on for new org units, then you should leave this property as an empty string, and the back-end service can populate it for you.
        /// </summary>
        public string? Path { get; set; }
        public IEnumerable<int> ParentOrgUnitIds { get; set; } = Enumerable.Empty<int>();
    }
}
