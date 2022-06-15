namespace HR.BrightspaceConnector.Features.OrgUnits
{
    /// <summary>
    /// Fundamental information for an org unit as reported by org unit service actions.
    /// </summary>
    public class OrgUnit
    {
        public int? Identifier { get; set; }
        public string? Name { get; set; }
        
        /// <summary>
        /// In rare cases, an org unit may have no code set for it; in that case, you may get null for the code on actions that retrieve this structure. 
        /// This is most likely to happen for the root organization org unit only.
        /// </summary>
        public string? Code { get; set; }
        public OrgUnitTypeInfo? Type { get; set; }
    }
}
