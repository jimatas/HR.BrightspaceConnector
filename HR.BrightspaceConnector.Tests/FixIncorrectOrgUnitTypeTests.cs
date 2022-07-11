using HR.BrightspaceConnector.Features.OrgUnits;
using HR.BrightspaceConnector.Features.OrgUnits.Decorators;
using HR.BrightspaceConnector.Features.OrgUnits.Queries;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using System;
using System.Threading.Tasks;

namespace HR.BrightspaceConnector.Tests
{
    [TestClass]
    public class FixIncorrectOrgUnitTypeTests
    {
        private readonly Mock<IApiClient> mockedApiClient = new();

        [TestMethod]
        public async Task GivenOrgUnitWithIncorrectTypeId_CorrectsIt()
        {
            // Arrange
            mockedApiClient.Setup(apiClient => apiClient.GetOrgUnitTypesAsync(default)).ReturnsAsync(new[] {
                new OrgUnitType { Code = "Instituut", Id = 102, },
                new OrgUnitType { Code = "Opleiding", Id = 103, },
                new OrgUnitType { Code = "Semester", Id = 104 }
            });

            var nextOrgUnit = new OrgUnitRecord
            {
                Code = "HR-FIT",
                Name = "Dienst FIT",
                Type = 100, // Incorrect, should be 102.
                TypeCode = "Instituut",
                Parents = new[] { 6606 },
                Path = null,
                SyncAction = 'c',
                SyncEventId = Random.Shared.Next(1, int.MaxValue),
                SyncExternalKey = null,
                SyncInternalKey = Guid.NewGuid().ToString()
            };

            FixIncorrectOrgUnitType handler = new(mockedApiClient.Object);

            // Act
            var result = await handler.HandleAsync(
                query: new GetNextOrgUnit(isDepartmentType: false),
                next: () => Task.FromResult<OrgUnitRecord?>(nextOrgUnit),
                cancellationToken: default);

            // Assert
            Assert.AreEqual(102, result!.Type);
        }

        [TestMethod]
        public async Task GivenOrgUnitWithMiscapitalizedTypeCode_CorrectsIt()
        {
            // Arrange
            mockedApiClient.Setup(apiClient => apiClient.GetOrgUnitTypesAsync(default)).ReturnsAsync(new[] {
                new OrgUnitType { Code = "Instituut", Id = 102, },
                new OrgUnitType { Code = "Opleiding", Id = 103, },
                new OrgUnitType { Code = "Semester", Id = 104 }
            });

            var nextOrgUnit = new OrgUnitRecord
            {
                Code = "HR-FIT",
                Name = "Dienst FIT",
                Type = 102,
                TypeCode = "instituut", // All lowercase, first letter should be capitalized.
                Parents = new[] { 6606 },
                Path = null,
                SyncAction = 'c',
                SyncEventId = Random.Shared.Next(1, int.MaxValue),
                SyncExternalKey = null,
                SyncInternalKey = Guid.NewGuid().ToString()
            };

            FixIncorrectOrgUnitType handler = new(mockedApiClient.Object);

            // Act
            var result = await handler.HandleAsync(
                query: new GetNextOrgUnit(isDepartmentType: false),
                next: () => Task.FromResult<OrgUnitRecord?>(nextOrgUnit),
                cancellationToken: default);

            // Assert
            Assert.AreEqual("Instituut", result!.TypeCode);
        }
    }
}
