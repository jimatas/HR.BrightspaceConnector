using HR.BrightspaceConnector.Features.OrgUnits;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using System.Linq;
using System.Threading.Tasks;

namespace HR.BrightspaceConnector.Tests
{
    [TestClass]
    public class OrgUnitRecordExtensionsTests
    {
        private readonly Mock<IApiClient> mockedApiClient = new();

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
        public async Task ToOrgUnitCreateDataAsync_ByDefault_CopiesAllProperties()
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

            mockedApiClient.Setup(apiClient => apiClient.GetOrgUnitTypesAsync(default)).ReturnsAsync(new[] {
                new OrgUnitType { Code = "Instituut", Id = 102, },
                new OrgUnitType { Code = "Opleiding", Id = 103, },
                new OrgUnitType { Code = "Semester", Id = 104 }
            });

            // Act
            var orgUnitCreateData = await orgUnitRecord.ToOrgUnitCreateDataAsync(mockedApiClient.Object);

            // Assert
            Assert.IsNotNull(orgUnitCreateData);
            Assert.AreEqual("HR-FIT", orgUnitCreateData.Code);
            Assert.AreEqual("Dienst FIT", orgUnitCreateData.Name);
            Assert.AreEqual(102, orgUnitCreateData.Type);
            Assert.IsTrue(orgUnitCreateData.Parents.All(id => id == 6606));
        }

        [TestMethod]
        public async Task ToOrgUnitCreateDataAsync_WithIncorrectOrgUnitTypeId_CorrectsIt()
        {
            // Arrange
            var orgUnitRecord = new OrgUnitRecord
            {
                Code = "HR-FIT",
                Name = "Dienst FIT",
                Type = 100, // Erroneous, should be 102.
                TypeCode = "Instituut",
                Parents = new[] { 6606 }
            };

            mockedApiClient.Setup(apiClient => apiClient.GetOrgUnitTypesAsync(default)).ReturnsAsync(new[] {
                new OrgUnitType { Code = "Instituut", Id = 102, },
                new OrgUnitType { Code = "Opleiding", Id = 103, },
                new OrgUnitType { Code = "Semester", Id = 104 }
            });

            // Act
            var orgUnitCreateData = await orgUnitRecord.ToOrgUnitCreateDataAsync(mockedApiClient.Object);

            // Assert
            Assert.AreEqual(102, orgUnitCreateData.Type);
        }
    }
}
