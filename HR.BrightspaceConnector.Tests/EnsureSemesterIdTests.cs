using HR.BrightspaceConnector.Features.Common;
using HR.BrightspaceConnector.Features.Courses;
using HR.BrightspaceConnector.Features.Courses.Decorators;
using HR.BrightspaceConnector.Features.Courses.Queries;
using HR.BrightspaceConnector.Features.OrgUnits;
using HR.BrightspaceConnector.Infrastructure;

using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace HR.BrightspaceConnector.Tests
{
    [TestClass]
    public class EnsureSemesterIdTests
    {
        private static readonly int SemesterId = Random.Shared.Next(1, int.MaxValue);
        
        private readonly Mock<IApiClient> mockedApiClient = new();
        private readonly PagedResultSet<OrgUnitProperties> pagedResultSet = new PagedResultSet<OrgUnitProperties>
        {
            Items = new[]
            {
                new OrgUnitProperties{
                    Code = "2021",
                    Identifier = SemesterId,
                    Name = "Dienst FIT",
                    Type = new OrgUnitTypeInfo
                    {
                        Code = "Semester",
                        Id = 104,
                        Name = "Semester (collegejaar)"
                    },
                    Path = "/content/Hogeschool_Rotterdam/2021"
                }
            },
            PagingInfo = new PagingInfo
            {
                Bookmark = SemesterId.ToString(),
                HasMoreItems = false
            }
        };

        [DataTestMethod]
        [DataRow(null)]
        [DataRow(0)]
        public async Task GivenCourseOfferingWithMissingSemesterId_LooksItUp(int? nullOrDefault)
        {
            // Arrange
            mockedApiClient.Setup(apiClient => apiClient.GetOrgUnitTypesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new[] {
                new OrgUnitType { Code = "Instituut", Id = 102, },
                new OrgUnitType { Code = "Opleiding", Id = 103, },
                new OrgUnitType { Code = "Semester", Id = 104 }
            });

            mockedApiClient.Setup(apiClient => apiClient.GetOrgUnitsAsync(
                It.Is<OrgUnitQueryParameters>(queryParams => queryParams.ExactOrgUnitCode == "2021" && queryParams.OrgUnitType == 104),
                It.IsAny<CancellationToken>())).ReturnsAsync(pagedResultSet);

            CourseOfferingRecord nextCourseOffering = new CourseOfferingRecord
            {
                Code = "HR-SampleCourseOffering",
                Name = "Sample course offering created by a unit test",
                CourseTemplateId = 1024,
                Description = "This course offering is a sample that was created by a unit test. You can safely ignore it.",
                IsActive = true,
                SemesterCode = "2021",
                SemesterId = nullOrDefault,
                SyncAction = 'c',
                SyncEventId = Random.Shared.Next(1, int.MaxValue),
                SyncExternalKey = null,
                SyncInternalKey = Guid.NewGuid().ToString()
            };

            var standardTypeCodes = new StandardOrgUnitTypeCodes
            {
                CustomOrgUnit = "Instituut",
                Department = "Opleiding",
                Semester = "Semester"
            };

            EnsureSemesterId handler = new(mockedApiClient.Object, Options.Create(standardTypeCodes));

            // Act
            var result = await handler.HandleAsync(
                query: new GetNextCourseOffering(),
                next: () => Task.FromResult<CourseOfferingRecord?>(nextCourseOffering),
                cancellationToken: default);

            // Assert
            Assert.AreEqual(SemesterId, result!.SemesterId);
        }
    }
}
