namespace HR.BrightspaceConnector.Features.Users
{
    /// <summary>
    /// When you use an action to update a user's data, you pass in a block like this.
    /// </summary>
    public class UpdateUserData
    {
        public string? OrgDefinedId { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? ExternalEmail { get; set; }
        public string? UserName { get; set; }
        public UserActivationData? Activation { get; set; }

        /// <summary>
        /// Added with LP API v1.33 as of LMS v20.21.9
        /// </summary>
        public string? Pronouns { get; set; }
    }
}
