namespace HR.BrightspaceConnector.Features.Users
{
    public static class UserRecordExtensions
    {
        public static CreateUserData ToCreateUserData(this UserRecord userRecord)
        {
            return new CreateUserData
            {
                OrgDefinedId = userRecord.OrgDefinedId,
                UserName = userRecord.UserName,
                FirstName = userRecord.FirstName,
                MiddleName = userRecord.MiddleName,
                LastName = userRecord.LastName,
                ExternalEmail = userRecord.ExternalEmail,
                RoleId = userRecord.RoleId,
                IsActive = userRecord.IsActive,
                SendCreationEmail = userRecord.SendCreationEmail
            };
        }

        public static UpdateUserData ToUpdateUserData(this UserRecord userRecord)
        {
            return new UpdateUserData
            {
                OrgDefinedId = userRecord.OrgDefinedId,
                UserName = userRecord.UserName,
                FirstName = userRecord.FirstName,
                MiddleName = userRecord.MiddleName,
                LastName = userRecord.LastName,
                ExternalEmail = userRecord.ExternalEmail,
                Activation = new UserActivationData { IsActive = userRecord.IsActive }
            };
        }
    }
}
