using HR.BrightspaceConnector.Features.Users;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HR.BrightspaceConnector.Tests
{
    internal static class AssertExtensions
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

        public static bool AreEqual(this Assert _, UpdateUserData expected, UpdateUserData actual)
        {
            Assert.AreEqual(expected.UserName, actual.UserName);
            Assert.AreEqual(expected.OrgDefinedId, actual.OrgDefinedId);
            Assert.AreEqual(expected.FirstName, actual.FirstName);
            Assert.AreEqual(expected.MiddleName, actual.MiddleName);
            Assert.AreEqual(expected.LastName, actual.LastName);
            Assert.AreEqual(expected.Pronouns, actual.Pronouns);
            Assert.AreEqual(expected.ExternalEmail, actual.ExternalEmail);
            Assert.AreEqual(expected.Activation?.IsActive, actual.Activation?.IsActive);

            return true;
        }

        public static bool AreEqual(this Assert _, LegalPreferredNames expected, LegalPreferredNames actual)
        {
            Assert.AreEqual(expected.LegalLastName, actual.LegalLastName);
            Assert.AreEqual(expected.LegalFirstName, actual.LegalFirstName);
            Assert.AreEqual(expected.SortLastName, actual.SortLastName);
            Assert.AreEqual(expected.PreferredLastName, actual.PreferredLastName);
            Assert.AreEqual(expected.PreferredFirstName, actual.PreferredFirstName);

            return true;
        }
    }
}
