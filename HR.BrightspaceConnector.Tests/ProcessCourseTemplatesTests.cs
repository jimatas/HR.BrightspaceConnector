﻿using HR.BrightspaceConnector.Features.Courses;
using HR.BrightspaceConnector.Features.Courses.Commands;
using HR.BrightspaceConnector.Infrastructure.Persistence;
using HR.Common.Cqrs.Commands;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using System;
using System.Threading.Tasks;

using CreateCourseTemplateData = HR.BrightspaceConnector.Features.Courses.CreateCourseTemplate;

namespace HR.BrightspaceConnector.Tests
{
    [TestClass]
    public class ProcessCourseTemplatesTests : DependencyInjectedTestsBase
    {
        private const int RootOrgId = 6606;

        private readonly Mock<IDatabase> mockedDatabase = new();
        private readonly Mock<IApiClient> mockedApiClient = new();

        [DataTestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task GivenNullCourseTemplateInAnyContext_DoesNothing(bool isDeleteContext)
        {
            // Arrange
            mockedDatabase.Setup(database => database.GetNextCourseTemplateAsync(default)).ReturnsAsync(null as CourseTemplateRecord);

            IServiceProvider serviceProvider = CreateServiceProvider(mockedDatabase.Object, mockedApiClient.Object);
            ICommandDispatcher commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();

            // Act
            await commandDispatcher.DispatchAsync(new ProcessCourseTemplates(batchSize: 1, isDeleteContext));

            // Assert
            mockedDatabase.Verify(database => database.GetNextCourseTemplateAsync(default), Times.Once());
            mockedDatabase.VerifyNoOtherCalls();
            mockedApiClient.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task GivenCourseTemplateToCreate_CreatesCourseTemplate()
        {
            // Arrange
            int eventId = Random.Shared.Next(1, int.MaxValue);
            int courseTemplateId = Random.Shared.Next(1, int.MaxValue);

            var courseTemplateRecord = new CourseTemplateRecord
            {
                Code = "HR-SampleCourseTemplate",
                Name = "Sample course template created by a unit test",
                ParentOrgUnitIds = new[] { RootOrgId },
                Path = null,
                SyncAction = 'c',
                SyncEventId = eventId,
                SyncExternalKey = null,
                SyncInternalKey = Guid.NewGuid().ToString()
            };

            mockedDatabase.Setup(database => database.GetNextCourseTemplateAsync(default)).ReturnsAsync(courseTemplateRecord);
            mockedDatabase.Setup(database => database.MarkAsHandledAsync(eventId, true, courseTemplateId, null, default));

            mockedApiClient.Setup(apiClient => apiClient.CreateCourseTemplateAsync(It.Is<CreateCourseTemplateData>(createCourseTemplate => Assert.That.AreEqual(courseTemplateRecord.ToCreateCourseTemplate(), createCourseTemplate)), default))
                .ReturnsAsync(new CourseTemplate
                {
                    Code = "HR-SampleCourseTemplate",
                    Identifier = courseTemplateId,
                    Name = "Sample course template created by a unit test",
                    Path = "/content/Hogeschool_Rotterdam/HR-SampleCourseTemplate"
                });

            IServiceProvider serviceProvider = CreateServiceProvider(mockedDatabase.Object, mockedApiClient.Object);
            ICommandDispatcher commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();

            // Act
            await commandDispatcher.DispatchAsync(new ProcessCourseTemplates(batchSize: 1, isDeleteContext: false));

            // Assert
            mockedDatabase.VerifyAll();
            mockedApiClient.VerifyAll();
        }
    }
}
