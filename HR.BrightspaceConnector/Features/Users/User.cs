namespace HR.BrightspaceConnector.Features.Users
{
    /// <summary>
    /// This structure gets used as a composite to include in resource blocks returned by a variety of services (for example, enrollments).
    /// </summary>
    public class User
    {
        public int? Identifier { get; set; }
        public string? DisplayName { get; set; }
        public string? EmailAddress { get; set; }
        public string? OrgDefinedId { get; set; }
        public Uri? ProfileBadgeUrl { get; set; }
        public string? ProfileIdentifier { get; set; }
    }
}
