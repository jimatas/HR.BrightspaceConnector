namespace HR.BrightspaceConnector.Features.OrgUnits
{
    /// <summary>
    /// Encapsulates the core information associated with an org unit type for use by other services (for example, the Enrollment and Course related actions).
    /// </summary>
    public class OrgUnitTypeInfo
    {
        public int? Id { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
    }
}
