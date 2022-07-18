using System.ComponentModel.DataAnnotations;

namespace HR.BrightspaceConnector.Features.Sections
{
    /// <summary>
    /// Serves as a base class for the <see cref="CreateOrUpdateSectionData"/> and <see cref="SectionData"/> classes.
    /// </summary>
    public abstract class SectionDataBase
    {
        public string? Name { get; set; }

        /// <summary>
        /// Note that section code values have these limitations:
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
