using HR.BrightspaceConnector.Features.Enrollments;
using HR.BrightspaceConnector.Features.Enrollments.Commands;
using HR.BrightspaceConnector.Infrastructure.Persistence;
using HR.Common.Cqrs.Commands;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HR.BrightspaceConnector.Tests
{
    [TestClass]
    public class ProcessEnrollmentsTests : DependencyInjectedTestsBase
    {
        private static readonly IDictionary<string, int> RoleIds = new Dictionary<string, int>()
        {
            { "Administrator", 117 },
            { "Instructor", 109 },
            { "Learner", 110 },
        };

        private readonly Mock<IDatabase> mockedDatabase = new();
        private readonly Mock<IApiClient> mockedApiClient = new();

        [DataTestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task GivenNullEnrollmentInAnyContext_DoesNothing(bool isDeleteContext)
        {
            // Arrange
            mockedDatabase.Setup(database => database.GetNextCourseOfferingEnrollmentAsync(default)).ReturnsAsync(null as EnrollmentRecord);

            IServiceProvider serviceProvider = CreateServiceProvider(mockedDatabase.Object, mockedApiClient.Object);
            ICommandDispatcher commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();

            // Act
            await commandDispatcher.DispatchAsync(new ProcessEnrollments(batchSize: 1, isDeleteContext));

            // Assert
            mockedDatabase.Verify(database => database.GetNextCourseOfferingEnrollmentAsync(default), Times.Once());
            mockedDatabase.VerifyNoOtherCalls();
            mockedApiClient.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task GivenEnrollmentToCreate_CreatesIt()
        {
            // Arrange
            int eventId = Random.Shared.Next(1, int.MaxValue);
            int orgUnitId = 1108;
            int userId = 2915;
            string externalKey = "8498333";

            var enrollmentRecord = new EnrollmentRecord
            {
                SyncAction = 'c',
                SyncEventId = eventId,
                SyncInternalKey = Random.Shared.Next(1, int.MaxValue).ToString(),
                OrgUnitId = orgUnitId,
                UserId = userId,
                RoleId = RoleIds["Learner"]
            };

            mockedDatabase.Setup(database => database.GetNextCourseOfferingEnrollmentAsync(default)).ReturnsAsync(enrollmentRecord);
            mockedDatabase.Setup(database => database.MarkAsHandledAsync(eventId, true, externalKey, null, default));

            mockedApiClient.Setup(apiClient => apiClient.CreateOrUpdateEnrollmentAsync(It.Is<CreateEnrollmentData>(createEnrollmentData => Assert.That.AreEqual(enrollmentRecord.ToCreateEnrollmentData(), createEnrollmentData)), default))
                .ReturnsAsync(new EnrollmentData
                {
                    OrgUnitId = orgUnitId,
                    RoleId = RoleIds["Learner"],
                    UserId = userId
                });

            IServiceProvider serviceProvider = CreateServiceProvider(mockedDatabase.Object, mockedApiClient.Object);
            ICommandDispatcher commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();

            // Act
            await commandDispatcher.DispatchAsync(new ProcessEnrollments(batchSize: 1, isDeleteContext: false));

            // Assert
            mockedDatabase.VerifyAll();
            mockedApiClient.VerifyAll();
        }

        [TestMethod]
        public async Task GivenEnrollmentToCreateInDeleteContext_DoesNothing()
        {
            // Arrange
            mockedDatabase.Setup(database => database.GetNextCourseOfferingEnrollmentAsync(default)).ReturnsAsync(
                new EnrollmentRecord
                {
                    SyncAction = 'c',
                    SyncEventId = Random.Shared.Next(1, int.MaxValue),
                    SyncInternalKey = Random.Shared.Next(1, int.MaxValue).ToString(),
                    OrgUnitId = Random.Shared.Next(1, int.MaxValue),
                    UserId = Random.Shared.Next(1, int.MaxValue),
                    RoleId = RoleIds["Learner"]
                });

            IServiceProvider serviceProvider = CreateServiceProvider(mockedDatabase.Object, mockedApiClient.Object);
            ICommandDispatcher commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();

            // Act
            await commandDispatcher.DispatchAsync(new ProcessEnrollments(batchSize: 1, isDeleteContext: true));

            // Assert
            mockedDatabase.Verify(database => database.GetNextCourseOfferingEnrollmentAsync(default), Times.Once());
            mockedDatabase.VerifyNoOtherCalls();
            mockedApiClient.VerifyNoOtherCalls();
        }
    }
}
