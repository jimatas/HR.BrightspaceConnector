namespace HR.BrightspaceConnector.Features.Users
{
    /// <summary>
    /// When you use an action to create a user, you pass in a block of new-user data like this.
    /// </summary>
    public class CreateUserData : UserDataBase
    {
        public int? RoleId { get; set; }
        public bool? IsActive { get; set; }
        public bool? SendCreationEmail { get; set; }
    }
}
