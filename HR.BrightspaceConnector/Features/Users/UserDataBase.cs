namespace HR.BrightspaceConnector.Features.Users
{
    /// <summary>
    /// Serves as a base class for the <see cref="UserData"/>, <see cref="CreateUserData"/> and <see cref="UpdateUserData"/> classes.
    /// </summary>
    public abstract class UserDataBase
    {
        public string? OrgDefinedId { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? UserName { get; set; }
        public string? ExternalEmail { get; set; }

        /// <summary>
        /// Added with LP API v1.33 as of LMS v20.21.9
        /// </summary>
        public string? Pronouns { get; set; }
    }
}
