using HR.BrightspaceConnector.Features.Enrollments;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;

namespace HR.BrightspaceConnector.Tests
{
    [TestClass]
    public class EnrollmentRecordExtensionsTests
    {
        [TestMethod]
        public void ToEnrollmentData_ByDefault_CopiesAllProperties()
        {
            // Arrange
            var enrollmentRecord = new EnrollmentRecord
            {
                OrgUnitId = 1108,
                UserId = 2915,
                RoleId = 110,
                SyncExternalKey = "8498333",
                SyncAction = 'u',
                SyncEventId = Random.Shared.Next(1, int.MaxValue),
                SyncInternalKey = Random.Shared.Next(1, int.MaxValue).ToString()
            };

            // Act
            EnrollmentData enrollmentData = enrollmentRecord.ToEnrollmentData();

            // Assert
            Assert.IsNotNull(enrollmentData);
            Assert.AreEqual(enrollmentRecord.OrgUnitId, enrollmentData.OrgUnitId);
            Assert.AreEqual(enrollmentRecord.UserId, enrollmentData.UserId);
            Assert.AreEqual(enrollmentRecord.RoleId, enrollmentData.RoleId);
            Assert.IsNull(enrollmentData.IsCascading);
        }

        [TestMethod]
        public void ToCreateEnrollmentData_ByDefault_CopiesAllProperties()
        {
            // Arrange
            var enrollmentRecord = new EnrollmentRecord
            {
                OrgUnitId = 1108,
                UserId = 2915,
                RoleId = 110,
                SyncExternalKey = null,
                SyncAction = 'c',
                SyncEventId = Random.Shared.Next(1, int.MaxValue),
                SyncInternalKey = Random.Shared.Next(1, int.MaxValue).ToString()
            };

            // Act
            CreateEnrollmentData createEnrollmentData = enrollmentRecord.ToCreateEnrollmentData();

            // Assert
            Assert.IsNotNull(createEnrollmentData);
            Assert.AreEqual(enrollmentRecord.OrgUnitId, createEnrollmentData.OrgUnitId);
            Assert.AreEqual(enrollmentRecord.UserId, createEnrollmentData.UserId);
            Assert.AreEqual(enrollmentRecord.RoleId, createEnrollmentData.RoleId);
        }
    }
}
