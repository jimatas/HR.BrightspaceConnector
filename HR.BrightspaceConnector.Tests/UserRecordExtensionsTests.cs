using HR.BrightspaceConnector.Features.Users;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HR.BrightspaceConnector.Tests
{
    [TestClass]
    public class UserRecordExtensionsTests
    {
        [TestMethod]
        public void ToCreateUserData_ByDefault_CopiesAllProperties()
        {
            // Arrange
            var userRecord = new UserRecord
            {
                OrgDefinedId = "d2l.demo.instructor",
                UserName = "Demo.Instructor",
                FirstName = "D2L.Demo",
                MiddleName = "",
                LastName = "Instructor",
                ExternalEmail = "demo.instructor@d2l.com",
                RoleId = 1,
                IsActive = true,
                SendCreationEmail = true,
            };

            // Act
            var createUserData = userRecord.ToCreateUserData();

            // Assert
            Assert.IsNotNull(createUserData);
            Assert.AreEqual("d2l.demo.instructor", createUserData.OrgDefinedId);
            Assert.AreEqual("Demo.Instructor", createUserData.UserName);
            Assert.AreEqual("D2L.Demo", createUserData.FirstName);
            Assert.AreEqual(string.Empty, createUserData.MiddleName);
            Assert.AreEqual("Instructor", createUserData.LastName);
            Assert.AreEqual("demo.instructor@d2l.com", createUserData.ExternalEmail);
            Assert.AreEqual(1, createUserData.RoleId);
            Assert.IsTrue(createUserData.IsActive);
            Assert.IsTrue(createUserData.SendCreationEmail);
        }

        [TestMethod]
        public void ToUpdateUserData_ByDefault_CopiesAllProperties()
        {
            // Arrange
            var userRecord = new UserRecord
            {
                OrgDefinedId = "d2l.demo.student",
                UserName = "Demo.Student",
                FirstName = "D2L.Demo",
                MiddleName = "",
                LastName = "Student",
                ExternalEmail = "demo.student@d2l.com",
                IsActive = true
            };

            // Act
            var updateUserData = userRecord.ToUpdateUserData();

            // Assert
            Assert.IsNotNull(updateUserData);
            Assert.AreEqual("d2l.demo.student", updateUserData.OrgDefinedId);
            Assert.AreEqual("Demo.Student", updateUserData.UserName);
            Assert.AreEqual("D2L.Demo", updateUserData.FirstName);
            Assert.AreEqual(string.Empty, updateUserData.MiddleName);
            Assert.AreEqual("Student", updateUserData.LastName);
            Assert.AreEqual("demo.student@d2l.com", updateUserData.ExternalEmail);
            Assert.IsTrue(updateUserData.Activation!.IsActive);
        }
    }
}
