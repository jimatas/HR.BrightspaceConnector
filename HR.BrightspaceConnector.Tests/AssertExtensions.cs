using HR.BrightspaceConnector.Features.Users;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HR.BrightspaceConnector.Tests
{
    public static class AssertExtensions
    {
        public static bool AreEqual(this Assert _, CreateUserData expected, CreateUserData actual)
        {
            Assert.AreEqual(expected.UserName, actual.UserName);
            Assert.AreEqual(expected.OrgDefinedId, actual.OrgDefinedId);
            Assert.AreEqual(expected.RoleId, actual.RoleId);
            Assert.AreEqual(expected.FirstName, actual.FirstName);
            Assert.AreEqual(expected.MiddleName, actual.MiddleName);
            Assert.AreEqual(expected.LastName, actual.LastName);
            Assert.AreEqual(expected.Pronouns, actual.Pronouns);
            Assert.AreEqual(expected.ExternalEmail, actual.ExternalEmail);
            Assert.AreEqual(expected.IsActive, actual.IsActive);
            Assert.AreEqual(expected.SendCreationEmail, actual.SendCreationEmail);

            return true;
        }
    }
}
