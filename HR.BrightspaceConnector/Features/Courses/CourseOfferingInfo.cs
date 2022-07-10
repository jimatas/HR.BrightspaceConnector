using HR.BrightspaceConnector.Features.Common;
using HR.BrightspaceConnector.Infrastructure;

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HR.BrightspaceConnector.Features.Courses
{
    /// <summary>
    /// When you use actions that update course information for a course offering, you should provide one of these blocks.
    /// </summary>
    public class CourseOfferingInfo
    {
        public string? Name { get; set; }

        /// <summary>
        /// Note that course offering code values have these limitations:
        /// <list type="bullet">
        ///   <item>They cannot be more than 50 characters in length.</item>
        ///   <item>
        ///     They may not contain these special characters (or a newline character):
        ///     <code>\ : * ? " &lt; &gt; | ' # , % &amp;</code>
        ///   </item>
        /// </list>
        /// </summary>
        [StringLength(maximumLength: 50)]
        
        public string? Code { get; set; }

        [JsonConverter(typeof(CustomDateTimeOffsetConverter))]
        public DateTimeOffset? StartDate { get; set; }

        [JsonConverter(typeof(CustomDateTimeOffsetConverter))]
        public DateTimeOffset? EndDate { get; set; }

        public bool? IsActive { get; set; }

        /// <summary>
        /// Note that this property is optional to include on input for LP API versions 1.25 and earlier; however, if you include it, the contents are ignored. 
        /// Note also that this property uses the RichTextInput structure type.
        /// Added with LP API v1.26
        /// </summary>
        public RichTextInput? Description { get; set; }

        /// <summary>
        /// Whether or not a user can self-register this course. Null is treated as false. 
        /// This is required as of API version of LP v1.27, but not supported on versions prior to v1.27.
        /// Added with LP API v1.27
        /// </summary>
        public bool? CanSelfRegister { get; set; }
    }
}
