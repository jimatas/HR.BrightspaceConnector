using HR.BrightspaceConnector.Features.OrgUnits;
using HR.BrightspaceConnector.Features.OrgUnits.Commands;
using HR.BrightspaceConnector.Features.OrgUnits.Decorators;
using HR.BrightspaceConnector.Infrastructure;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace HR.BrightspaceConnector.Tests
{
    [TestClass]
    public class EnsureOrgUnitPathTests
    {
        private static readonly int OrgUnitId = Random.Shared.Next(1, int.MaxValue);

        private readonly Mock<IApiClient> mockedApiClient = new();
        private readonly PagedResultSet<OrgUnitProperties> pagedResultSet = new PagedResultSet<OrgUnitProperties>
        {
            Items = new[]
            {
                new OrgUnitProperties{
                    Code = "HR-FIT",
                    Identifier = OrgUnitId,
                    Name = "Dienst FIT",
                    Type = new OrgUnitTypeInfo
                    {
                        Code = "Instituut",
                        Id = 102,
                        Name = "Instituut (custom)"
                    },
                    Path = "/content/Hogeschool_Rotterdam/HR-FIT"
                }
            },
            PagingInfo = new PagingInfo
            {
                Bookmark = OrgUnitId.ToString(),
                HasMoreItems = false
            }
        };

        [TestMethod]
        public async Task GivenOrgUnitWithPathSet_DoesNothing()
        {
            // Arrange
            mockedApiClient.Setup(apiClient => apiClient.GetOrgUnitsAsync(
                It.Is<OrgUnitQueryParameters>(queryParams => queryParams.ExactOrgUnitCode == "HR-FIT"),
                It.IsAny<CancellationToken>())).ReturnsAsync(pagedResultSet);

            EnsureOrgUnitPath handler = new(mockedApiClient.Object);
            UpdateOrgUnit command = new(new OrgUnitRecord
            {
                Code = "HR-FIT",
                Name = "Dienst FIT",
                Type = 102,
                TypeCode = "Instituut",
                Parents = new[] { 6606 },
                Path = "/content/Hogeschool_Rotterdam/HR-FIT/Extra_Segment",
                SyncAction = 'u',
                SyncExternalKey = OrgUnitId.ToString()
            });

            // Act
            await handler.HandleAsync(command, () => Task.CompletedTask, default);

            // Assert
            Assert.AreEqual("/content/Hogeschool_Rotterdam/HR-FIT/Extra_Segment", command.OrgUnit.Path);
            mockedApiClient.VerifyNoOtherCalls();
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        public async Task GivenOrgUnitWithMissingPath_SetsPath(string path)
        {
            // Arrange
            mockedApiClient.Setup(apiClient => apiClient.GetOrgUnitsAsync(
                It.Is<OrgUnitQueryParameters>(queryParams => queryParams.ExactOrgUnitCode == "HR-FIT"),
                It.IsAny<CancellationToken>())).ReturnsAsync(pagedResultSet);

            EnsureOrgUnitPath handler = new(mockedApiClient.Object);
            UpdateOrgUnit command = new(new OrgUnitRecord
            {
                Code = "HR-FIT",
                Name = "Dienst FIT",
                Type = 102,
                TypeCode = "Instituut",
                Parents = new[] { 6606 },
                Path = path,
                SyncAction = 'u',
                SyncExternalKey = OrgUnitId.ToString()
            });

            // Act
            await handler.HandleAsync(command, () => Task.CompletedTask, default);

            // Assert
            Assert.AreEqual("/content/Hogeschool_Rotterdam/HR-FIT", command.OrgUnit.Path);
        }
    }
}
