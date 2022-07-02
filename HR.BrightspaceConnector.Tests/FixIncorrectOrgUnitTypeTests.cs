using HR.BrightspaceConnector.Features.OrgUnits;
using HR.BrightspaceConnector.Features.OrgUnits.Commands;
using HR.BrightspaceConnector.Features.OrgUnits.Wrappers;

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
        public async Task GivenOrgUnitToCreateWithIncorrectTypeId_CorrectsIt()
        {
            // Arrange
            mockedApiClient.Setup(apiClient => apiClient.GetOrgUnitTypesAsync(default)).ReturnsAsync(new[] {
                new OrgUnitType { Code = "Instituut", Id = 102, },
                new OrgUnitType { Code = "Opleiding", Id = 103, },
                new OrgUnitType { Code = "Semester", Id = 104 }
            });

            FixIncorrectOrgUnitType handler = new(mockedApiClient.Object);
            CreateOrgUnit command = new(new OrgUnitRecord
            {
                Code = "HR-FIT",
                Name = "Dienst FIT",
                Type = 100, // Incorrect, should be 102.
                TypeCode = "Instituut",
                Parents = new[] { 6606 },
                SyncAction = 'c'
            });

            // Act
            await handler.HandleAsync(command, () => Task.CompletedTask, default);

            // Assert
            Assert.AreEqual(102, command.OrgUnit.Type);
        }

        [TestMethod]
        public async Task GivenOrgUnitToCreateWithMiscapitalizedTypeCode_CorrectsIt()
        {
            // Arrange
            mockedApiClient.Setup(apiClient => apiClient.GetOrgUnitTypesAsync(default)).ReturnsAsync(new[] {
                new OrgUnitType { Code = "Instituut", Id = 102, },
                new OrgUnitType { Code = "Opleiding", Id = 103, },
                new OrgUnitType { Code = "Semester", Id = 104 }
            });

            FixIncorrectOrgUnitType handler = new(mockedApiClient.Object);
            CreateOrgUnit command = new(new OrgUnitRecord
            {
                Code = "HR-FIT",
                Name = "Dienst FIT",
                Type = 102,
                TypeCode = "instituut", // All lowercase, should be capitalized.
                Parents = new[] { 6606 },
                SyncAction = 'c'
            });

            // Act
            await handler.HandleAsync(command, () => Task.CompletedTask, default);

            // Assert
            Assert.AreEqual("Instituut", command.OrgUnit.TypeCode);
        }

        [TestMethod]
        public async Task GivenOrgUnitToUpdateWithIncorrectTypeId_CorrectsIt()
        {
            // Arrange
            mockedApiClient.Setup(apiClient => apiClient.GetOrgUnitTypesAsync(default)).ReturnsAsync(new[] {
                new OrgUnitType { Code = "Instituut", Id = 102, },
                new OrgUnitType { Code = "Opleiding", Id = 103, },
                new OrgUnitType { Code = "Semester", Id = 104 }
            });

            FixIncorrectOrgUnitType handler = new(mockedApiClient.Object);
            UpdateOrgUnit command = new(new OrgUnitRecord
            {
                Code = "HR-FIT",
                Name = "Dienst FIT",
                Type = 100, // Incorrect, should be 102.
                TypeCode = "Instituut",
                Parents = new[] { 6606 },
                SyncAction = 'u',
                SyncExternalKey = Random.Shared.Next(1, int.MaxValue).ToString()
            });

            // Act
            await handler.HandleAsync(command, () => Task.CompletedTask, default);

            // Assert
            Assert.AreEqual(102, command.OrgUnit.Type);
        }

        [TestMethod]
        public async Task GivenOrgUnitToUpdateWithMiscapitalizedTypeCode_CorrectsIt()
        {
            // Arrange
            mockedApiClient.Setup(apiClient => apiClient.GetOrgUnitTypesAsync(default)).ReturnsAsync(new[] {
                new OrgUnitType { Code = "Instituut", Id = 102, },
                new OrgUnitType { Code = "Opleiding", Id = 103, },
                new OrgUnitType { Code = "Semester", Id = 104 }
            });

            FixIncorrectOrgUnitType handler = new(mockedApiClient.Object);
            UpdateOrgUnit command = new(new OrgUnitRecord
            {
                Code = "HR-FIT",
                Name = "Dienst FIT",
                Type = 102,
                TypeCode = "instituut", // All lowercase, should be capitalized.
                Parents = new[] { 6606 },
                SyncAction = 'u',
                SyncExternalKey = Random.Shared.Next(1, int.MaxValue).ToString()
            });

            // Act
            await handler.HandleAsync(command, () => Task.CompletedTask, default);

            // Assert
            Assert.AreEqual("Instituut", command.OrgUnit.TypeCode);
        }
    }
}
