using HR.BrightspaceConnector.Features.Common;
using HR.BrightspaceConnector.Features.Courses;
using HR.BrightspaceConnector.Features.Courses.Commands;
using HR.BrightspaceConnector.Infrastructure.Persistence;
using HR.Common.Cqrs.Commands;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using System;
using System.Threading.Tasks;

using CreateCourseOfferingData = HR.BrightspaceConnector.Features.Courses.CreateCourseOffering;

namespace HR.BrightspaceConnector.Tests
{
    [TestClass]
    public class ProcessCourseOfferingsTests : DependencyInjectedTestsBase
    {
        private readonly Mock<IDatabase> mockedDatabase = new();
        private readonly Mock<IApiClient> mockedApiClient = new();

        [DataTestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task GivenNullCourseOfferingInAnyContext_DoesNothing(bool isDeleteContext)
        {
            // Arrange
            mockedDatabase.Setup(database => database.GetNextCourseOfferingAsync(default)).ReturnsAsync(null as CourseOfferingRecord);

            IServiceProvider serviceProvider = CreateServiceProvider(mockedDatabase.Object, mockedApiClient.Object);
            ICommandDispatcher commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();

            // Act
            await commandDispatcher.DispatchAsync(new ProcessCourseOfferings(batchSize: 1, isDeleteContext));

            // Assert
            mockedDatabase.Verify(database => database.GetNextCourseOfferingAsync(default), Times.Once());
            mockedDatabase.VerifyNoOtherCalls();
            mockedApiClient.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task GivenCourseOfferingToCreate_CreatesIt()
        {
            // Arrange
            int eventId = Random.Shared.Next(1, int.MaxValue);
            int courseOfferingId = Random.Shared.Next(1, int.MaxValue);
            int courseTemplateId = Random.Shared.Next(1, int.MaxValue);

            var courseOfferingRecord = new CourseOfferingRecord
            {
                Code = "HR-SampleCourseOffering",
                Name = "Sample course offering created by a unit test",
                CourseTemplateId = courseTemplateId,
                Description = "This course offering is a sample that was created by a unit test. You can safely ignore it.",
                SemesterCode = "2021",
                SemesterId = Random.Shared.Next(1, int.MaxValue),
                SyncAction = 'c',
                SyncEventId = eventId,
                SyncInternalKey = Guid.NewGuid().ToString()
            };

            mockedDatabase.Setup(database => database.GetNextCourseOfferingAsync(default)).ReturnsAsync(courseOfferingRecord);
            mockedDatabase.Setup(database => database.MarkAsHandledAsync(eventId, true, courseOfferingId.ToString(), null, default));

            mockedApiClient.Setup(apiClient => apiClient.CreateCourseOfferingAsync(It.Is<CreateCourseOfferingData>(createCourseOffering => Assert.That.AreEqual(courseOfferingRecord.ToCreateCourseOffering(), createCourseOffering)), default))
                .ReturnsAsync(new CourseOffering
                {
                    Identifier = courseOfferingId,
                    Code = "HR-SampleCourseOffering",
                    Name = "Sample course offering created by a unit test",
                    CourseTemplate = new BasicOrgUnit
                    {
                        Code = "HR-SampleCourseTemplate",
                        Identifier = Random.Shared.Next(1, int.MaxValue),
                        Name = "Sample course template created by a unit test"
                    },
                    Description = new RichText { Html = "This course offering is a sample that was created by a unit test. You can safely ignore it." },
                    Semester = new BasicOrgUnit
                    {
                        Code = "2021",
                        Identifier = Random.Shared.Next(1, int.MaxValue),
                        Name = "Collegejaar 2021"
                    }
                });

            IServiceProvider serviceProvider = CreateServiceProvider(mockedDatabase.Object, mockedApiClient.Object);
            ICommandDispatcher commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();

            // Act
            await commandDispatcher.DispatchAsync(new ProcessCourseOfferings(batchSize: 1, isDeleteContext: false));

            // Assert
            mockedDatabase.VerifyAll();
            mockedApiClient.VerifyAll();
        }

        [TestMethod]
        public async Task GivenCourseOfferingToCreateInDeleteContext_DoesNothing()
        {
            // Arrange
            mockedDatabase.Setup(database => database.GetNextCourseOfferingAsync(default)).ReturnsAsync(new CourseOfferingRecord
            {
                Code = "HR-SampleCourseOffering",
                Name = "Sample course offering created by a unit test",
                CourseTemplateId = Random.Shared.Next(1, int.MaxValue),
                Description = "This course offering is a sample that was created by a unit test. You can safely ignore it.",
                SemesterCode = "2021",
                SemesterId = Random.Shared.Next(1, int.MaxValue),
                SyncAction = 'c',
                SyncEventId = Random.Shared.Next(1, int.MaxValue),
                SyncInternalKey = Guid.NewGuid().ToString()
            });

            IServiceProvider serviceProvider = CreateServiceProvider(mockedDatabase.Object, mockedApiClient.Object);
            ICommandDispatcher commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();

            // Act
            await commandDispatcher.DispatchAsync(new ProcessCourseOfferings(batchSize: 1, isDeleteContext: true));

            // Assert
            mockedDatabase.Verify(database => database.GetNextCourseOfferingAsync(default), Times.Once());
            mockedDatabase.VerifyNoOtherCalls();
            mockedApiClient.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task GivenCourseOfferingToDelete_DeletesIt()
        {
            // Arrange
            int eventId = Random.Shared.Next(1, int.MaxValue);
            int courseOfferingId = Random.Shared.Next(1, int.MaxValue);

            mockedDatabase.Setup(database => database.GetNextCourseOfferingAsync(default)).ReturnsAsync(new CourseOfferingRecord
            {
                SyncAction = 'd',
                SyncEventId = eventId,
                SyncExternalKey = courseOfferingId.ToString(),
                SyncInternalKey = Guid.NewGuid().ToString()
            });
            mockedDatabase.Setup(database => database.MarkAsHandledAsync(eventId, true, courseOfferingId.ToString(), null, default));

            mockedApiClient.Setup(apiClient => apiClient.DeleteCourseOfferingAsync(courseOfferingId, true, default));

            IServiceProvider serviceProvider = CreateServiceProvider(mockedDatabase.Object, mockedApiClient.Object);
            ICommandDispatcher commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();

            // Act
            await commandDispatcher.DispatchAsync(new ProcessCourseOfferings(batchSize: 1, isDeleteContext: true));

            // Assert
            mockedDatabase.VerifyAll();
            mockedApiClient.VerifyAll();
        }

        [TestMethod]
        public async Task GivenCourseOfferingToDeleteInNonDeleteContext_DoesNothing()
        {
            // Arrange
            mockedDatabase.Setup(database => database.GetNextCourseOfferingAsync(default)).ReturnsAsync(new CourseOfferingRecord
            {
                SyncAction = 'd',
                SyncEventId = Random.Shared.Next(1, int.MaxValue),
                SyncExternalKey = Random.Shared.Next(1, int.MaxValue).ToString(),
                SyncInternalKey = Guid.NewGuid().ToString()
            });

            IServiceProvider serviceProvider = CreateServiceProvider(mockedDatabase.Object, mockedApiClient.Object);
            ICommandDispatcher commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();

            // Act
            await commandDispatcher.DispatchAsync(new ProcessCourseOfferings(batchSize: 1, isDeleteContext: false));

            // Assert
            mockedDatabase.Verify(database => database.GetNextCourseOfferingAsync(default), Times.Once());
            mockedDatabase.VerifyNoOtherCalls();
            mockedApiClient.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task GivenCourseOfferingToUpdate_UpdatesIt()
        {
            // Arrange
            int eventId = Random.Shared.Next(1, int.MaxValue);
            int courseOfferingId = Random.Shared.Next(1, int.MaxValue);

            var courseOfferingRecord = new CourseOfferingRecord
            {
                Code = "HR-SampleCourseOffering",
                Name = "Sample course offering updated by a unit test",
                CourseTemplateId = Random.Shared.Next(1, int.MaxValue),
                Description = "This course offering is a sample that was created and then updated by a unit test. You can safely ignore it.",
                SemesterCode = "2021",
                SemesterId = Random.Shared.Next(1, int.MaxValue),
                SyncAction = 'u',
                SyncEventId = eventId,
                SyncExternalKey = courseOfferingId.ToString(),
                SyncInternalKey = Guid.NewGuid().ToString()
            };

            mockedDatabase.Setup(database => database.GetNextCourseOfferingAsync(default)).ReturnsAsync(courseOfferingRecord);
            mockedDatabase.Setup(database => database.MarkAsHandledAsync(eventId, true, courseOfferingId.ToString(), null, default));

            mockedApiClient.Setup(apiClient => apiClient.UpdateCourseOfferingAsync(courseOfferingId,
                It.Is<CourseOfferingInfo>(courseOfferingInfo => Assert.That.AreEqual(courseOfferingRecord.ToCourseOfferingInfo(), courseOfferingInfo)), default));

            IServiceProvider serviceProvider = CreateServiceProvider(mockedDatabase.Object, mockedApiClient.Object);
            ICommandDispatcher commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();

            // Act
            await commandDispatcher.DispatchAsync(new ProcessCourseOfferings(batchSize: 1, isDeleteContext: false));

            // Assert
            mockedDatabase.VerifyAll();
            mockedApiClient.VerifyAll();
        }

        [TestMethod]
        public async Task GivenCourseOfferingToUpdateInDeleteContext_DoesNothing()
        {
            // Arrange
            mockedDatabase.Setup(database => database.GetNextCourseOfferingAsync(default)).ReturnsAsync(new CourseOfferingRecord
            {
                Code = "HR-SampleCourseOffering",
                Name = "Sample course offering updated by a unit test",
                CourseTemplateId = Random.Shared.Next(1, int.MaxValue),
                Description = "This course offering is a sample that was created and then updated by a unit test. You can safely ignore it.",
                SemesterCode = "2021",
                SemesterId = Random.Shared.Next(1, int.MaxValue),
                SyncAction = 'u',
                SyncEventId = Random.Shared.Next(1, int.MaxValue),
                SyncExternalKey = Random.Shared.Next(1, int.MaxValue).ToString(),
                SyncInternalKey = Guid.NewGuid().ToString()
            });

            IServiceProvider serviceProvider = CreateServiceProvider(mockedDatabase.Object, mockedApiClient.Object);
            ICommandDispatcher commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();

            // Act
            await commandDispatcher.DispatchAsync(new ProcessCourseOfferings(batchSize: 1, isDeleteContext: true));

            // Assert
            mockedDatabase.Verify(database => database.GetNextCourseOfferingAsync(default), Times.Once());
            mockedDatabase.VerifyNoOtherCalls();
            mockedApiClient.VerifyNoOtherCalls();
        }
    }
}
