using HR.BrightspaceConnector.Features.Sections;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;

namespace HR.BrightspaceConnector.Tests
{
    [TestClass]
    public class SectionRecordExtensionsTests
    {
        [TestMethod]
        public void ToCreateOrUpdateSectionData_ByDefault_CopiesAllProperties()
        {
            // Arrange
            var sectionRecord = new SectionRecord
            {
                Code = "Section-1",
                Name = "Section 1",
                Description = "This is the first section in the course offering.",
                OrgUnitId = Random.Shared.Next(1, int.MaxValue),
                SyncAction = 'c',
                SyncEventId = Random.Shared.Next(1, int.MaxValue),
                SyncExternalKey = null,
                SyncInternalKey = Random.Shared.Next(1, int.MaxValue).ToString()
            };

            // Act
            var sectionCreateData = sectionRecord.ToCreateOrUpdateSectionData();

            // Assert
            Assert.IsNotNull(sectionCreateData);
            Assert.AreEqual(sectionRecord.Code, sectionCreateData.Code);
            Assert.AreEqual(sectionRecord.Description, sectionCreateData.Description?.Content);
            Assert.AreEqual(sectionRecord.Name, sectionCreateData.Name);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void ToCreateOrUpdateSectionData_GivenSectionRecordWithoutDescription_LeavesItOut(string? emptyValue)
        {
            // Arrange
            var sectionRecord = new SectionRecord
            {
                Code = "Section-1",
                Name = "Section 1",
                Description = emptyValue,
                OrgUnitId = Random.Shared.Next(1, int.MaxValue),
                SyncAction = 'c',
                SyncEventId = Random.Shared.Next(1, int.MaxValue),
                SyncExternalKey = null,
                SyncInternalKey = Random.Shared.Next(1, int.MaxValue).ToString()
            };

            // Act
            var sectionCreateData = sectionRecord.ToCreateOrUpdateSectionData();

            // Assert
            Assert.IsNull(sectionCreateData.Description);
        }
    }
}
