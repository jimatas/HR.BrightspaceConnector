using HR.BrightspaceConnector.Infrastructure;

using System.Text.Json.Serialization;

namespace HR.BrightspaceConnector.Features.Users
{
    /// <summary>
    /// When you use an action with the User Management service to retrieve a user's data, the service passes you back a data block 
    /// like this (notice that it's different to the User.WhoAmIUser information block provided by the WhoAmI service actions)
    /// </summary>
    public class UserData : User
    {
        public int? OrgId { get; set; }
        public int? UserId { get; set; }
        public string? UniqueIdentifier { get; set; }
        public UserActivationData? Activation { get; set; }

        [JsonConverter(typeof(CustomDateTimeOffsetConverter))]
        public DateTimeOffset? LastAccessedDate { get; set; }

        public string? DisplayName { get; set; }
    }
}
