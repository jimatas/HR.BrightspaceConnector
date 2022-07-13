using HR.BrightspaceConnector.Features.Courses;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;

namespace HR.BrightspaceConnector.Tests
{
    [TestClass]
    public class CourseOfferingRecordExtensionsTests
    {
        [TestMethod]
        public void ToCreateCourseOffering_ByDefault_CopiesAllProperties()
        {
            // Arrange
            var courseOfferingRecord = new CourseOfferingRecord
            {
                CanSelfRegister = true,
                Code = "HR-SampleCourseOffering",
                CourseTemplateId = 1024,
                Description = "This course offering is a sample that was created by a unit test. You can safely ignore it.",
                EndDate = new DateTime(2022, 07, 13).AddMonths(6),
                ForceLocale = true,
                IsActive = true,
                LocaleId = 31,
                Name = "Sample course offering created by a unit test",
                Path = "/content/Hogeschool_Rotterdam/HR-SampleCourseOffering",
                SemesterCode = "2021",
                SemesterId = 2021,
                ShowAddressBook = true,
                StartDate = new DateTime(2022, 07, 13),
                SyncAction = 'c',
                SyncEventId = 1002,
                SyncExternalKey = null,
                SyncInternalKey = "HR-SampleCourseOffering"
            };

            // Act
            CreateCourseOffering createCourseOffering = courseOfferingRecord.ToCreateCourseOffering();

            // Assert
            Assert.IsNotNull(createCourseOffering);
            Assert.AreEqual(courseOfferingRecord.CanSelfRegister, createCourseOffering.CanSelfRegister);
            Assert.AreEqual(courseOfferingRecord.Code, createCourseOffering.Code);
            Assert.AreEqual(courseOfferingRecord.CourseTemplateId, createCourseOffering.CourseTemplateId);
            Assert.AreEqual(courseOfferingRecord.Description, createCourseOffering.Description?.Content);
            Assert.AreEqual(courseOfferingRecord.EndDate, createCourseOffering.EndDate);
            Assert.AreEqual(courseOfferingRecord.ForceLocale, createCourseOffering.ForceLocale);
            Assert.AreEqual(courseOfferingRecord.IsActive, createCourseOffering.IsActive);
            Assert.AreEqual(courseOfferingRecord.LocaleId, createCourseOffering.LocaleId);
            Assert.AreEqual(courseOfferingRecord.Name, createCourseOffering.Name);
            Assert.AreEqual(courseOfferingRecord.Path, createCourseOffering.Path);
            Assert.AreEqual(courseOfferingRecord.SemesterId, createCourseOffering.SemesterId);
            Assert.AreEqual(courseOfferingRecord.ShowAddressBook, createCourseOffering.ShowAddressBook);
            Assert.AreEqual(courseOfferingRecord.StartDate, createCourseOffering.StartDate);
        }

        [TestMethod]
        public void ToCourseOfferingInfo_ByDefault_CopiesAllProperties()
        {
            // Arrange
            var courseOfferingRecord = new CourseOfferingRecord
            {
                CanSelfRegister = true,
                Code = "HR-SampleCourseOffering",
                CourseTemplateId = 1024,
                Description = "This course offering is a sample that was created by a unit test. You can safely ignore it.",
                EndDate = new DateTime(2022, 07, 13).AddMonths(6),
                ForceLocale = true,
                IsActive = true,
                LocaleId = 31,
                Name = "Sample course offering created by a unit test",
                Path = "/content/Hogeschool_Rotterdam/HR-SampleCourseOffering",
                SemesterCode = "2021",
                SemesterId = 2021,
                ShowAddressBook = true,
                StartDate = new DateTime(2022, 07, 13),
                SyncAction = 'c',
                SyncEventId = 1002,
                SyncExternalKey = null,
                SyncInternalKey = "HR-SampleCourseOffering"
            };

            // Act
            CourseOfferingInfo courseOfferingInfo = courseOfferingRecord.ToCourseOfferingInfo();

            // Assert
            Assert.IsNotNull(courseOfferingInfo);
            Assert.AreEqual(courseOfferingRecord.CanSelfRegister, courseOfferingInfo.CanSelfRegister);
            Assert.AreEqual(courseOfferingRecord.Code, courseOfferingInfo.Code);
            Assert.AreEqual(courseOfferingRecord.Description, courseOfferingInfo.Description?.Content);
            Assert.AreEqual(courseOfferingRecord.EndDate, courseOfferingInfo.EndDate);
            Assert.AreEqual(courseOfferingRecord.IsActive, courseOfferingInfo.IsActive);
            Assert.AreEqual(courseOfferingRecord.Name, courseOfferingInfo.Name);
            Assert.AreEqual(courseOfferingRecord.StartDate, courseOfferingInfo.StartDate);
        }
    }
}
