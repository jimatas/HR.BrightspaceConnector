using HR.BrightspaceConnector.Features.Common;
using HR.BrightspaceConnector.Features.Sections;
using HR.BrightspaceConnector.Features.Sections.Commands;
using HR.BrightspaceConnector.Infrastructure.Persistence;
using HR.Common.Cqrs.Commands;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using System;
using System.Threading.Tasks;

namespace HR.BrightspaceConnector.Tests
{
    [TestClass]
    public class ProcessSectionsTests : DependencyInjectedTestsBase
    {
        private readonly Mock<IDatabase> mockedDatabase = new();
        private readonly Mock<IApiClient> mockedApiClient = new();

        [DataTestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task GivenNullSectionInAnyContext_DoesNothing(bool isDeleteContext)
        {
            // Arrange
            mockedDatabase.Setup(database => database.GetNextCourseOfferingSectionAsync(default)).ReturnsAsync(null as SectionRecord);

            IServiceProvider serviceProvider = CreateServiceProvider(mockedDatabase.Object, mockedApiClient.Object);
            ICommandDispatcher commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();

            // Act
            await commandDispatcher.DispatchAsync(new ProcessSections(batchSize: 1, isDeleteContext));

            // Assert
            mockedDatabase.Verify(database => database.GetNextCourseOfferingSectionAsync(default), Times.Once());
            mockedDatabase.VerifyNoOtherCalls();
            mockedApiClient.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task GivenSectionToCreate_CreatesIt()
        {
            // Arrange
            int eventId = Random.Shared.Next(1, int.MaxValue);
            int orgUnitId = Random.Shared.Next(1, int.MaxValue);
            int sectionId = Random.Shared.Next(1, int.MaxValue);

            var sectionRecord = new SectionRecord
            {
                Code = "Section-1",
                Description = "Section 1, created by a unit test.",
                Name = "Section 1",
                OrgUnitId = orgUnitId,
                SyncAction = 'c',
                SyncEventId = eventId,
                SyncExternalKey = null,
                SyncInternalKey = Random.Shared.Next(1, int.MaxValue).ToString()
            };

            mockedDatabase.Setup(database => database.GetNextCourseOfferingSectionAsync(default)).ReturnsAsync(sectionRecord);
            mockedDatabase.Setup(database => database.MarkAsHandledAsync(eventId, true, sectionId.ToString(), null, default));

            mockedApiClient.Setup(apiClient => apiClient.CreateSectionAsync(orgUnitId, It.Is<CreateOrUpdateSectionData>(createSectionData => Assert.That.AreEqual(sectionRecord.ToCreateOrUpdateSectionData(), createSectionData)), default))
                .ReturnsAsync(new SectionData
                {
                    Code = "Section-1",
                    Description = new RichText
                    {
                        Html = "Section 1, created by a unit test."
                    },
                    Name = "Section 1",
                    SectionId = sectionId
                });

            IServiceProvider serviceProvider = CreateServiceProvider(mockedDatabase.Object, mockedApiClient.Object);
            ICommandDispatcher commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();

            // Act
            await commandDispatcher.DispatchAsync(new ProcessSections(batchSize: 1, isDeleteContext: false));

            // Assert
            mockedDatabase.VerifyAll();
            mockedApiClient.VerifyAll();
        }

        [TestMethod]
        public async Task GivenSectionToCreateInDeleteContext_DoesNothing()
        {
            // Arrange
            mockedDatabase.Setup(database => database.GetNextCourseOfferingSectionAsync(default)).ReturnsAsync(
                new SectionRecord
                {
                    Code = "Section-1",
                    Description = "Section 1, created by a unit test.",
                    Name = "Section 1",
                    OrgUnitId = Random.Shared.Next(1, int.MaxValue),
                    SyncAction = 'c',
                    SyncEventId = Random.Shared.Next(1, int.MaxValue),
                    SyncExternalKey = null,
                    SyncInternalKey = Random.Shared.Next(1, int.MaxValue).ToString()
                });

            IServiceProvider serviceProvider = CreateServiceProvider(mockedDatabase.Object, mockedApiClient.Object);
            ICommandDispatcher commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();

            // Act
            await commandDispatcher.DispatchAsync(new ProcessSections(batchSize: 1, isDeleteContext: true));

            // Assert
            mockedDatabase.Verify(database => database.GetNextCourseOfferingSectionAsync(default), Times.Once());
            mockedDatabase.VerifyNoOtherCalls();
            mockedApiClient.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task GivenSectionToDelete_DeletesIt()
        {
            // Arrange
            int eventId = Random.Shared.Next(1, int.MaxValue);
            int orgUnitId = Random.Shared.Next(1, int.MaxValue);
            int sectionId = Random.Shared.Next(1, int.MaxValue);

            var sectionRecord = new SectionRecord
            {
                OrgUnitId = orgUnitId,
                SyncAction = 'd',
                SyncEventId = eventId,
                SyncExternalKey = sectionId.ToString(),
                SyncInternalKey = Random.Shared.Next(1, int.MaxValue).ToString()
            };

            mockedDatabase.Setup(database => database.GetNextCourseOfferingSectionAsync(default)).ReturnsAsync(sectionRecord);
            mockedDatabase.Setup(database => database.MarkAsHandledAsync(eventId, true, sectionId.ToString(), null, default));

            mockedApiClient.Setup(apiClient => apiClient.DeleteSectionAsync(orgUnitId, sectionId, default));

            IServiceProvider serviceProvider = CreateServiceProvider(mockedDatabase.Object, mockedApiClient.Object);
            ICommandDispatcher commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();

            // Act
            await commandDispatcher.DispatchAsync(new ProcessSections(batchSize: 1, isDeleteContext: true));

            // Assert
            mockedDatabase.VerifyAll();
            mockedApiClient.VerifyAll();
        }

        [TestMethod]
        public async Task GivenSectionToDeleteInNonDeleteContext_DoesNothing()
        {
            // Arrange
            mockedDatabase.Setup(database => database.GetNextCourseOfferingSectionAsync(default))
                .ReturnsAsync(new SectionRecord
                {
                    OrgUnitId = Random.Shared.Next(1, int.MaxValue),
                    SyncAction = 'd',
                    SyncEventId = Random.Shared.Next(1, int.MaxValue),
                    SyncExternalKey = Random.Shared.Next(1, int.MaxValue).ToString(),
                    SyncInternalKey = Random.Shared.Next(1, int.MaxValue).ToString()
                });

            IServiceProvider serviceProvider = CreateServiceProvider(mockedDatabase.Object, mockedApiClient.Object);
            ICommandDispatcher commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();

            // Act
            await commandDispatcher.DispatchAsync(new ProcessSections(batchSize: 1, isDeleteContext: false));

            // Assert
            mockedDatabase.Verify(database => database.GetNextCourseOfferingSectionAsync(default), Times.Once());
            mockedDatabase.VerifyNoOtherCalls();
            mockedApiClient.VerifyNoOtherCalls();
        }
    }
}
