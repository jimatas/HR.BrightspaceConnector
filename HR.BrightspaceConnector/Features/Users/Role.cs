namespace HR.BrightspaceConnector.Features.Users
{
    /// <summary>
    /// This block describes a user role that you can assign to an enrolled user.
    /// </summary>
    public class Role
    {
        public int? Identifier { get; set; }
        public string? DisplayName { get; set; }
        public string? Code { get; set; }
    }
}
