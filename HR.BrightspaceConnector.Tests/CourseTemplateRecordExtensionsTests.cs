using HR.BrightspaceConnector.Features.Courses;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Linq;

namespace HR.BrightspaceConnector.Tests
{
    [TestClass]
    public class CourseTemplateRecordExtensionsTests
    {
        [TestMethod]
        public void ToCreateCourseTemplate_ByDefault_CopiesAllProperties()
        {
            // Arrange
            var courseTemplateRecord = new CourseTemplateRecord
            {
                Code = "HR-SampleCourseTemplate",
                Name = "Sample course template created by a unit test",
                ParentOrgUnitIds = new[] { 6606 },
                Path = "/content/Hogeschool_Rotterdam/HR-SampleCourseTemplate",
                SyncAction = 'c',
                SyncEventId = 1002,
                SyncExternalKey = null,
                SyncInternalKey = "HR-SampleCourseTemplate"
            };

            // Act
            CreateCourseTemplate createCourseTemplate = courseTemplateRecord.ToCreateCourseTemplate();

            // Assert
            Assert.IsNotNull(createCourseTemplate);
            Assert.AreEqual(courseTemplateRecord.Code, createCourseTemplate.Code);
            Assert.AreEqual(courseTemplateRecord.Name, createCourseTemplate.Name);
            CollectionAssert.AreEquivalent(courseTemplateRecord.ParentOrgUnitIds.ToList(), createCourseTemplate.ParentOrgUnitIds.ToList());
            Assert.AreEqual(courseTemplateRecord.Path, createCourseTemplate.Path);
        }

        [TestMethod]
        public void ToCourseTemplateInfo_ByDefault_CopiesAllProperties()
        {
            // Arrange
            var courseTemplateRecord = new CourseTemplateRecord
            {
                Code = "HR-SampleCourseTemplate",
                Name = "Sample course template created by a unit test",
                ParentOrgUnitIds = new[] { 6606 },
                Path = "/content/Hogeschool_Rotterdam/HR-SampleCourseTemplate",
                SyncAction = 'u',
                SyncEventId = 1002,
                SyncExternalKey = "54321",
                SyncInternalKey = "HR-SampleCourseTemplate"
            };

            // Act
            CourseTemplateInfo courseTemplateInfo = courseTemplateRecord.ToCourseTemplateInfo();

            // Assert
            Assert.IsNotNull(courseTemplateInfo);
            Assert.AreEqual(courseTemplateRecord.Code, courseTemplateInfo.Code);
            Assert.AreEqual(courseTemplateRecord.Name, courseTemplateInfo.Name);
        }
    }
}
