namespace HR.BrightspaceConnector.Features.OrgUnits
{
    /// <summary>
    /// Serves as a base class for the <see cref="OrgUnit"/> and <see cref="OrgUnitCreateData"/> classes.
    /// </summary>
    public abstract class OrgUnitBase
    {
        public string? Name { get; set; }

        /// <summary>
        /// In rare cases, an org unit may have no code set for it; in that case, you may get null for the code on actions that retrieve this structure. 
        /// This is most likely to happen for the root organization org unit only.
        /// </summary>
        public string? Code { get; set; }
    }
}
