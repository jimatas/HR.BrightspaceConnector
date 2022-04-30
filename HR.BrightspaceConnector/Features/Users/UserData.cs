namespace HR.BrightspaceConnector.Features.Users
{
    /// <summary>
    /// When you use an action with the User Management service to retrieve a user’s data, the service passes you back a data block 
    /// like this (notice that it’s different to the User.WhoAmIUser information block provided by the WhoAmI service actions)
    /// </summary>
    public class UserData
    {
        public int? OrgId { get; set; }
        public int? UserId { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? UserName { get; set; }
        public string? ExternalEmail { get; set; }
        public string? OrgDefinedId { get; set; }
        public string? UniqueIdentifier { get; set; }
        public UserActivationData? Activation { get; set; }
        public DateTimeOffset? LastAccessedDate { get; set; }
        public string? DisplayName { get; set; }

        /// <summary>
        /// Added with LP API v1.33
        /// </summary>
        public string? Pronouns { get; set; }
    }
}
