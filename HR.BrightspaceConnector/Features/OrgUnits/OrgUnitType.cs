namespace HR.BrightspaceConnector.Features.OrgUnits
{
    /// <summary>
    /// Actions that retrieve OrgUnitType information from the service receive blocks that look like this.
    /// </summary>
    public class OrgUnitType
    {
        public int? Id { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? SortOrder { get; set; }
        public Permissions? Permissions { get; set; }
    }
}
