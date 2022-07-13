namespace HR.BrightspaceConnector.Features.Enrollments
{
    /// <summary>
    /// Structure for the enrolled user's information that the service exposes through the classlist API.
    /// </summary>
    public class ClasslistUser
    {
        public int? Identifier { get; set; }
        public string? ProfileIdentifier { get; set; }
        public string? DisplayName { get; set; }
        public string? UserName { get; set; }
        public string? OrgDefinedId { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int? RoleId { get; set; }
        public DateTimeOffset? LastAccessed { get; set; }
        public bool? IsOnline { get; set; }
    }
}
