namespace HR.BrightspaceConnector.Features.Users
{
    /// <summary>
    /// When you use an action to update a user's data, you pass in a block like this.
    /// </summary>
    public class UpdateUserData : User
    {
        public UserActivationData? Activation { get; set; }
    }
}
