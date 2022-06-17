namespace HR.BrightspaceConnector.Features.Users
{
    public abstract class User
    {
        public string? OrgDefinedId { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? UserName { get; set; }
        public string? ExternalEmail { get; set; }

        /// <summary>
        /// Added with LP API v1.33
        /// </summary>
        public string? Pronouns { get; set; }
    }
}
