namespace HR.BrightspaceConnector.Features.OrgUnits
{
    /// <summary>
    /// Fundamental information for the organization itself (the root org unit).
    /// </summary>
    public class Organization
    {
        public int? Identifier { get; set; }
        public string? Name { get; set; }

        /// <summary>
        /// Configured local time zone for the back-end Brightspace service.
        /// </summary>
        public string? TimeZone { get; set; }
    }
}
