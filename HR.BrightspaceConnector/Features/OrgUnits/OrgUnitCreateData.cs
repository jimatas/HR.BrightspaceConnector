using System.ComponentModel.DataAnnotations;

namespace HR.BrightspaceConnector.Features.OrgUnits
{
    /// <summary>
    /// Actions that create or update custom org units should provide data to the service in blocks that look like this.
    /// </summary>
    public class OrgUnitCreateData
    {
        /// <summary>
        /// D2LID for the org unit's associated org unit type.
        /// </summary>
        public int? Type { get; set; }
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

        /// <summary>
        /// JSON array of Org unit IDs that identify this org unit's immediate parent org units.
        /// </summary>
        public IEnumerable<int> Parents { get; set; } = Enumerable.Empty<int>();
    }
}
