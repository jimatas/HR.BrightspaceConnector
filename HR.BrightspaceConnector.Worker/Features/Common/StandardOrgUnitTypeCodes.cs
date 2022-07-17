namespace HR.BrightspaceConnector.Features.Common
{
    /// <summary>
    /// Defines the RUAS specific type codes that can be used to identify the standard org unit types.
    /// </summary>
    public class StandardOrgUnitTypeCodes
    {
        /// <summary>
        /// The type code of the Department org unit type. 
        /// Default value, if not configured otherwise, is "Opleiding".
        /// </summary>
        public string Department { get; set; } = "Opleiding";

        /// <summary>
        /// The type code of the custom org unit that represents a RUAS institute.
        /// Default value, if not configured otherwise, is "Instituut".
        /// </summary>
        public string CustomOrgUnit { get; set; } = "Instituut";

        /// <summary>
        /// The type code of the Semester org unit type.
        /// Default value, if not configured otherwise, is "Collegejaar".
        /// </summary>
        public string Semester { get; set; } = "Collegejaar";
    }
}
