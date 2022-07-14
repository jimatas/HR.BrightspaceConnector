using HR.BrightspaceConnector.Features.Common;

namespace HR.BrightspaceConnector.Features.Courses
{
    /// <summary>
    /// When you use actions that update course information for a course offering, you should provide one of these blocks.
    /// </summary>
    public class CourseOfferingInfo : CourseOfferingBase
    {
        public bool? IsActive { get; set; }

        /// <summary>
        /// Note that this property is optional to include on input for LP API versions 1.25 and earlier; however, if you include it, the contents are ignored. 
        /// Note also that this property uses the RichTextInput structure type.
        /// Added with LP API v1.26
        /// </summary>
        public RichTextInput? Description { get; set; }
    }
}
