namespace HR.BrightspaceConnector.Features.Users
{
    /// <summary>
    /// When you use an action to create a user, you pass in a block of new-user data.
    /// </summary>
    public class CreateUserData
    {
        public string? OrgDefinedId { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? ExternalEmail { get; set; }
        public string? UserName { get; set; }
        public int? RoleId { get; set; }
        public bool? IsActive { get; set; }
        public bool? SendCreationEmail { get; set; }

        /// <summary>
        /// Added with LP API v1.33 as of LMS v20.21.9
        /// </summary>
        public string? Pronouns { get; set; }
    }
}
