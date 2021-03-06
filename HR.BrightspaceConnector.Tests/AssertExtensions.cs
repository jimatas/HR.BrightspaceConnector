using HR.BrightspaceConnector.Features.Courses;
using HR.BrightspaceConnector.Features.Enrollments;
using HR.BrightspaceConnector.Features.OrgUnits;
using HR.BrightspaceConnector.Features.Sections;
using HR.BrightspaceConnector.Features.Users;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Linq;

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

        public static bool AreEqual(this Assert _, OrgUnitCreateData expected, OrgUnitCreateData actual)
        {
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.Code, actual.Code);
            Assert.AreEqual(expected.Type, actual.Type);
            CollectionAssert.AreEquivalent(expected.Parents.ToList(), actual.Parents.ToList());

            return true;
        }

        public static bool AreEqual(this Assert _, OrgUnitProperties expected, OrgUnitProperties actual)
        {
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.Code, actual.Code);
            Assert.AreEqual(expected.Type?.Code, actual.Type?.Code);
            Assert.AreEqual(expected.Type?.Id, actual.Type?.Id);
            Assert.AreEqual(expected.Type?.Name, actual.Type?.Name);
            Assert.AreEqual(expected.Path, actual.Path);

            return true;
        }

        public static bool AreEqual(this Assert _, CreateCourseTemplate expected, CreateCourseTemplate actual)
        {
            Assert.That.AreEqual((CourseTemplateInfo)expected, actual);
            Assert.AreEqual(expected.Path, actual.Path);
            CollectionAssert.AreEqual(expected.ParentOrgUnitIds.ToList(), actual.ParentOrgUnitIds.ToList());

            return true;
        }

        public static bool AreEqual(this Assert _, CourseTemplateInfo expected, CourseTemplateInfo actual)
        {
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.Code, actual.Code);

            return true;
        }

        private static bool AreEqual(this Assert _, CourseOfferingBase expected, CourseOfferingBase actual)
        {
            Assert.AreEqual(expected.CanSelfRegister, actual.CanSelfRegister);
            Assert.AreEqual(expected.Code, actual.Code);
            Assert.AreEqual(expected.EndDate, actual.EndDate);
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.StartDate, actual.StartDate);

            return true;
        }

        public static bool AreEqual(this Assert _, CreateCourseOffering expected, CreateCourseOffering actual)
        {
            Assert.That.AreEqual((CourseOfferingBase)expected, actual);
            Assert.AreEqual(expected.CourseTemplateId, actual.CourseTemplateId);
            Assert.AreEqual(expected.Description?.Content, actual.Description?.Content);
            Assert.AreEqual(expected.Description?.Type, actual.Description?.Type);
            Assert.AreEqual(expected.ForceLocale, actual.ForceLocale);
            Assert.AreEqual(expected.LocaleId, actual.LocaleId);
            Assert.AreEqual(expected.Path, actual.Path);
            Assert.AreEqual(expected.SemesterId, actual.SemesterId);
            Assert.AreEqual(expected.ShowAddressBook, actual.ShowAddressBook);

            return true;
        }

        public static bool AreEqual(this Assert _, CourseOfferingInfo expected, CourseOfferingInfo actual)
        {
            Assert.That.AreEqual((CourseOfferingBase)expected, actual);
            Assert.AreEqual(expected.Description?.Content, actual.Description?.Content);
            Assert.AreEqual(expected.Description?.Type, actual.Description?.Type);
            Assert.AreEqual(expected.IsActive, actual.IsActive);

            return true;
        }

        public static bool AreEqual(this Assert _, CreateEnrollmentData expected, CreateEnrollmentData actual)
        {
            Assert.AreEqual(expected.OrgUnitId, actual.OrgUnitId);
            Assert.AreEqual(expected.RoleId, actual.RoleId);
            Assert.AreEqual(expected.UserId, actual.UserId);

            return true;
        }

        public static bool AreEqual(this Assert _, CreateOrUpdateSectionData expected, CreateOrUpdateSectionData actual)
        {
            Assert.AreEqual(expected.Code, actual.Code);
            Assert.AreEqual(expected.Description?.Content, actual.Description?.Content);
            Assert.AreEqual(expected.Description?.Type, actual.Description?.Type);
            Assert.AreEqual(expected.Name, actual.Name);

            return true;
        }
    }
}
