namespace HR.BrightspaceConnector.Features.Users
{
    public class UserRecord
    {
        public int? SyncEventId { get; set; }
        public char? SyncAction { get; set; }
        public string? SyncExternalKey { get; set; }
        public string? SyncInternalKey { get; set; }
        public string? UserName { get; set; }
        public string? OrgDefinedId { get; set; }
        public int? RoleId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? MiddleName { get; set; }
        public string? ExternalEmail { get; set; }
        public bool? IsActive { get; set; }
        public bool? SendCreationEmail { get; set; }
    }
}
