using System.ComponentModel.DataAnnotations;

namespace HR.BrightspaceConnector.Features.Courses
{
    public class CourseTemplateInfo
    {
        public string? Name { get; set; }

        /// <summary>
        /// Note that org unit code values have these limitations:
        /// <list type="bullet">
        ///   <item>They cannot be more than 50 characters in length.</item>
        ///   <item>
        ///     They may not contain these special characters:
        ///     <code>\ : * ? “ ” &lt; &gt; | ‘  # , % &amp;</code>
        ///   </item>
        /// </list>
        /// </summary>
        [StringLength(maximumLength: 50)]
        public string? Code { get; set; }
    }
}
