namespace HR.BrightspaceConnector.Features.OrgUnits
{
    /// <summary>
    /// Fundamental information for an org unit as reported by org unit service actions.
    /// </summary>
    public class OrgUnit : OrgUnitBase
    {
        public int? Identifier { get; set; }
        public OrgUnitTypeInfo? Type { get; set; }
    }
}
