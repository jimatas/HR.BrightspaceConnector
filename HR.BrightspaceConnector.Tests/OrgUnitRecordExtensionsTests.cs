using HR.BrightspaceConnector.Features.OrgUnits;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Linq;

namespace HR.BrightspaceConnector.Tests
{
    [TestClass]
    public class OrgUnitRecordExtensionsTests
    {
        [TestMethod]
        public void ToOrgUnitCreateData_ByDefault_CopiesAllProperties()
        {
            // Arrange
            var orgUnitRecord = new OrgUnitRecord
            {
                Code = "HR-FIT",
                Name = "Dienst FIT",
                Type = 102,
                TypeCode = "Instituut",
                Parents = new[] { 6606 }
            };

            // Act
            var orgUnitCreateData = orgUnitRecord.ToOrgUnitCreateData();

            // Assert
            Assert.IsNotNull(orgUnitCreateData);
            Assert.AreEqual("HR-FIT", orgUnitCreateData.Code);
            Assert.AreEqual("Dienst FIT", orgUnitCreateData.Name);
            Assert.AreEqual(102, orgUnitCreateData.Type);
            Assert.IsTrue(orgUnitCreateData.Parents.All(id => id == 6606));
        }

        [TestMethod]
        public void ToOrgUnitProperties_ByDefault_CopiesAllProperties()
        {
            // Arrange
            var orgUnitRecord = new OrgUnitRecord
            {
                Code = "HR-FIT",
                Name = "Dienst FIT",
                Type = 102,
                TypeCode = "Instituut",
                Parents = new[] { 6606 }
            };

            // Act
            var orgUnitProperties = orgUnitRecord.ToOrgUnitProperties();

            // Assert
            Assert.IsNotNull(orgUnitProperties);
            Assert.AreEqual("HR-FIT", orgUnitProperties.Code);
            Assert.AreEqual("Dienst FIT", orgUnitProperties.Name);
            Assert.IsNotNull(orgUnitProperties.Type);
            Assert.AreEqual(102, orgUnitProperties.Type.Id);
            Assert.AreEqual("Instituut", orgUnitProperties.Type.Code);
        }
    }
}
