using HR.BrightspaceConnector.Features.OrgUnits;
using HR.BrightspaceConnector.Features.OrgUnits.Commands;
using HR.BrightspaceConnector.Infrastructure.Persistence;
using HR.Common.Cqrs.Commands;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace HR.BrightspaceConnector.Tests
{
    [TestClass]
    public class ProcessOrgUnitsTests : DependencyInjectedTestsBase
    {
        private const int RootOrgId = 6606;

        private readonly Mock<IDatabase> mockedDatabase = new();
        private readonly Mock<IApiClient> mockedApiClient = new();

        [DataTestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task GivenNullCustomOrgUnitRegardlessOfContext_DoesNothing(bool isDeleteContext)
        {
            // Arrange
            mockedDatabase.Setup(database => database.GetNextCustomOrgUnitAsync(default)).ReturnsAsync(null as OrgUnitRecord);

            IServiceProvider serviceProvider = CreateServiceProvider(mockedDatabase.Object, mockedApiClient.Object);
            ICommandDispatcher commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();

            // Act
            await commandDispatcher.DispatchAsync(new ProcessOrgUnits(batchSize: 1, isDepartmentType: false, isDeleteContext));

            // Assert
            mockedDatabase.Verify(database => database.GetNextCustomOrgUnitAsync(It.IsAny<CancellationToken>()), Times.Once());
            mockedDatabase.VerifyNoOtherCalls();

            mockedApiClient.VerifyNoOtherCalls();
        }

        [DataTestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task GivenNullDepartmentRegardlessOfContext_DoesNothing(bool isDeleteContext)
        {
            // Arrange
            mockedDatabase.Setup(database => database.GetNextDepartmentAsync(default)).ReturnsAsync(null as OrgUnitRecord);

            IServiceProvider serviceProvider = CreateServiceProvider(mockedDatabase.Object, mockedApiClient.Object);
            ICommandDispatcher commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();

            // Act
            await commandDispatcher.DispatchAsync(new ProcessOrgUnits(batchSize: 1, isDepartmentType: true, isDeleteContext));

            // Assert
            mockedDatabase.Verify(database => database.GetNextDepartmentAsync(It.IsAny<CancellationToken>()), Times.Once());
            mockedDatabase.VerifyNoOtherCalls();

            mockedApiClient.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task GivenCustomOrgUnitToCreate_CreatesOrgUnit()
        {
            // Arrange
            int eventId = Random.Shared.Next(1, int.MaxValue);
            int orgUnitId = Random.Shared.Next(1, int.MaxValue);

            var orgUnitRecord = new OrgUnitRecord
            {
                Code = "HR-FIT",
                Name = "Dienst FIT",
                Parents = new[] { RootOrgId },
                SyncEventId = eventId,
                SyncAction = 'c',
                SyncInternalKey = Guid.NewGuid().ToString(),
                Type = 102
            };

            mockedDatabase.Setup(database => database.GetNextCustomOrgUnitAsync(default)).ReturnsAsync(orgUnitRecord);
            mockedDatabase.Setup(database => database.MarkAsHandledAsync(eventId, true, orgUnitId, null, default));

            mockedApiClient.Setup(apiClient => apiClient.CreateOrgUnitAsync(It.Is<OrgUnitCreateData>(orUnitCreateData => Assert.That.AreEqual(orgUnitRecord.ToOrgUnitCreateData(), orUnitCreateData)), default)).ReturnsAsync(() => new OrgUnit
            {
                Code = "HR-FIT",
                Identifier = orgUnitId,
                Name = "Dienst FIT",
                Type = new OrgUnitTypeInfo
                {
                    Code = "Instituten (custom)",
                    Id = 102,
                    Name = "Instituten (custom)"
                }
            });

            IServiceProvider serviceProvider = CreateServiceProvider(mockedDatabase.Object, mockedApiClient.Object);
            ICommandDispatcher commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();

            // Act
            await commandDispatcher.DispatchAsync(new ProcessOrgUnits(batchSize: 1, isDeleteContext: false));

            // Assert
            mockedDatabase.VerifyAll();
            mockedApiClient.VerifyAll();
        }

        [TestMethod]
        public async Task GivenDepartmentToCreate_CreatesOrgUnit()
        {
            // Arrange
            int eventId = Random.Shared.Next(1, int.MaxValue);
            int orgUnitId = Random.Shared.Next(1, int.MaxValue);
            int parentOrgUnitId = Random.Shared.Next(1, int.MaxValue);

            var orgUnitRecord = new OrgUnitRecord
            {
                Code = "HR-FIT-TAB",
                Name = "Afdeling applicatiebeheer",
                Parents = new[] { parentOrgUnitId },
                SyncEventId = eventId,
                SyncAction = 'c',
                SyncInternalKey = Guid.NewGuid().ToString(),
                Type = 101
            };

            mockedDatabase.Setup(database => database.GetNextDepartmentAsync(default)).ReturnsAsync(orgUnitRecord);
            mockedDatabase.Setup(database => database.MarkAsHandledAsync(eventId, true, orgUnitId, null, default));

            mockedApiClient.Setup(apiClient => apiClient.CreateOrgUnitAsync(It.Is<OrgUnitCreateData>(orUnitCreateData => Assert.That.AreEqual(orgUnitRecord.ToOrgUnitCreateData(), orUnitCreateData)), default)).ReturnsAsync(() => new OrgUnit
            {
                Code = "HR-FIT",
                Identifier = orgUnitId,
                Name = "Dienst FIT",
                Type = new OrgUnitTypeInfo
                {
                    Code = "Instituten (custom)",
                    Id = 102,
                    Name = "Instituten (custom)"
                }
            });

            IServiceProvider serviceProvider = CreateServiceProvider(mockedDatabase.Object, mockedApiClient.Object);
            ICommandDispatcher commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();

            // Act
            await commandDispatcher.DispatchAsync(new ProcessOrgUnits(batchSize: 1, isDepartmentType: true, isDeleteContext: false));

            // Assert
            mockedDatabase.VerifyAll();
            mockedApiClient.VerifyAll();
        }

        [TestMethod]
        public async Task GivenCustomOrgUnitToCreateInDeleteContext_DoesNothing()
        {
            // Arrange
            mockedDatabase.Setup(database => database.GetNextCustomOrgUnitAsync(default)).ReturnsAsync(new OrgUnitRecord
            {
                Code = "HR-FIT",
                Name = "Dienst FIT",
                Parents = new[] { RootOrgId },
                SyncEventId = Random.Shared.Next(1, int.MaxValue),
                SyncAction = 'c',
                SyncInternalKey = Guid.NewGuid().ToString(),
                Type = 102
            });

            IServiceProvider serviceProvider = CreateServiceProvider(mockedDatabase.Object, mockedApiClient.Object);
            ICommandDispatcher commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();

            // Act
            await commandDispatcher.DispatchAsync(new ProcessOrgUnits(batchSize: 1, isDeleteContext: true));

            // Assert
            mockedDatabase.Verify(database => database.GetNextCustomOrgUnitAsync(It.IsAny<CancellationToken>()), Times.Once());
            mockedDatabase.VerifyNoOtherCalls();

            mockedApiClient.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task GivenCustomOrgUnitToDelete_DeletesOrgUnit()
        {
            // Arrange
            var eventId = Random.Shared.Next(1, int.MaxValue);
            var orgUnitId = Random.Shared.Next(1, int.MaxValue);

            mockedDatabase.Setup(database => database.GetNextCustomOrgUnitAsync(default)).ReturnsAsync(new OrgUnitRecord
            {
                SyncEventId = eventId,
                SyncAction = 'd',
                SyncExternalKey = orgUnitId.ToString(),
                SyncInternalKey = Guid.NewGuid().ToString()
            });
            mockedDatabase.Setup(database => database.MarkAsHandledAsync(eventId, true, orgUnitId, null, default));

            mockedApiClient.Setup(apiClient => apiClient.DeleteOrgUnitAsync(orgUnitId, true, default));

            IServiceProvider serviceProvider = CreateServiceProvider(mockedDatabase.Object, mockedApiClient.Object);
            ICommandDispatcher commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();

            // Act
            await commandDispatcher.DispatchAsync(new ProcessOrgUnits(batchSize: 1, isDeleteContext: true));

            // Assert
            mockedDatabase.VerifyAll();
            mockedApiClient.VerifyAll();
        }

        [TestMethod]
        public async Task GivenDepartmentToDelete_DeletesOrgUnit()
        {
            // Arrange
            var eventId = Random.Shared.Next(1, int.MaxValue);
            var orgUnitId = Random.Shared.Next(1, int.MaxValue);

            mockedDatabase.Setup(database => database.GetNextDepartmentAsync(default)).ReturnsAsync(new OrgUnitRecord
            {
                SyncEventId = eventId,
                SyncAction = 'd',
                SyncExternalKey = orgUnitId.ToString(),
                SyncInternalKey = Guid.NewGuid().ToString()
            });
            mockedDatabase.Setup(database => database.MarkAsHandledAsync(eventId, true, orgUnitId, null, default));

            mockedApiClient.Setup(apiClient => apiClient.DeleteOrgUnitAsync(orgUnitId, true, default));

            IServiceProvider serviceProvider = CreateServiceProvider(mockedDatabase.Object, mockedApiClient.Object);
            ICommandDispatcher commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();

            // Act
            await commandDispatcher.DispatchAsync(new ProcessOrgUnits(batchSize: 1, isDepartmentType: true, isDeleteContext: true));

            // Assert
            mockedDatabase.VerifyAll();
            mockedApiClient.VerifyAll();
        }

        [TestMethod]
        public async Task GivenCustomOrgUnitToDeleteInNonDeleteContext_DoesNothing()
        {
            // Arrange
            var eventId = Random.Shared.Next(1, int.MaxValue);
            var orgUnitId = Random.Shared.Next(1, int.MaxValue);

            mockedDatabase.Setup(database => database.GetNextCustomOrgUnitAsync(default)).ReturnsAsync(new OrgUnitRecord
            {
                SyncEventId = eventId,
                SyncAction = 'd',
                SyncExternalKey = orgUnitId.ToString(),
                SyncInternalKey = Guid.NewGuid().ToString()
            });
            mockedDatabase.Setup(database => database.MarkAsHandledAsync(eventId, true, orgUnitId, null, default));

            mockedApiClient.Setup(apiClient => apiClient.DeleteOrgUnitAsync(orgUnitId, true, default));

            IServiceProvider serviceProvider = CreateServiceProvider(mockedDatabase.Object, mockedApiClient.Object);
            ICommandDispatcher commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();

            // Act
            await commandDispatcher.DispatchAsync(new ProcessOrgUnits(batchSize: 1, isDeleteContext: false));

            // Assert
            mockedDatabase.Verify(database => database.GetNextCustomOrgUnitAsync(It.IsAny<CancellationToken>()), Times.Once());
            mockedDatabase.VerifyNoOtherCalls();

            mockedApiClient.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task GivenCustomOrgUnitToUpdate_UpdatesOrgUnit()
        {
            // Arrange
            var eventId = Random.Shared.Next(1, int.MaxValue);
            var orgUnitId = Random.Shared.Next(1, int.MaxValue);

            var orgUnitRecord = new OrgUnitRecord
            {
                Code = "HR-FIT",
                Name = "Dienst FIT",
                Parents = new[] { RootOrgId },
                SyncEventId = eventId,
                SyncAction = 'u',
                SyncExternalKey = orgUnitId.ToString(),
                SyncInternalKey = Guid.NewGuid().ToString(),
                Type = 102
            };

            mockedDatabase.Setup(database => database.GetNextCustomOrgUnitAsync(default)).ReturnsAsync(orgUnitRecord);
            mockedDatabase.Setup(database => database.MarkAsHandledAsync(eventId, true, orgUnitId, null, default));

            mockedApiClient.Setup(apiClient => apiClient.UpdateOrgUnitAsync(orgUnitId, It.Is<OrgUnitProperties>(orgUnitProperties => Assert.That.AreEqual(orgUnitRecord.ToOrgUnitProperties(), orgUnitProperties)), default)).ReturnsAsync(() => new OrgUnitProperties
            {
                Code = "HR-FIT",
                Identifier = orgUnitId,
                Name = "Dienst FIT",
                Type = new OrgUnitTypeInfo
                {
                    Code = "Instituten (custom)",
                    Id = 102,
                    Name = "Instituten (custom)"
                },
                Path = "/content/Hogeschool_Rotterdam/HR-FIT"
            });

            IServiceProvider serviceProvider = CreateServiceProvider(mockedDatabase.Object, mockedApiClient.Object);
            ICommandDispatcher commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();

            // Act
            await commandDispatcher.DispatchAsync(new ProcessOrgUnits(batchSize: 1, isDeleteContext: false));

            // Assert
            mockedDatabase.VerifyAll();
            mockedApiClient.VerifyAll();
        }

        [TestMethod]
        public async Task GivenDepartmentToUpdate_UpdatesOrgUnit()
        {
            // Arrange
            var eventId = Random.Shared.Next(1, int.MaxValue);
            var orgUnitId = Random.Shared.Next(1, int.MaxValue);
            var parentOrgUnitId = Random.Shared.Next(1, int.MaxValue);

            var orgUnitRecord = new OrgUnitRecord
            {
                Code = "HR-FIT-TAB",
                Name = "Afdeling applicatiebeheer",
                Parents = new[] { parentOrgUnitId },
                SyncEventId = eventId,
                SyncAction = 'u',
                SyncExternalKey = orgUnitId.ToString(),
                SyncInternalKey = Guid.NewGuid().ToString(),
                Type = 101
            };

            mockedDatabase.Setup(database => database.GetNextDepartmentAsync(default)).ReturnsAsync(orgUnitRecord);
            mockedDatabase.Setup(database => database.MarkAsHandledAsync(eventId, true, orgUnitId, null, default));

            mockedApiClient.Setup(apiClient => apiClient.UpdateOrgUnitAsync(orgUnitId, It.Is<OrgUnitProperties>(orgUnitProperties => Assert.That.AreEqual(orgUnitRecord.ToOrgUnitProperties(), orgUnitProperties)), default)).ReturnsAsync(() => new OrgUnitProperties
            {
                Code = "HR-FIT-TAB",
                Identifier = orgUnitId,
                Name = "Afdeling applicatiebeheer",
                Type = new OrgUnitTypeInfo
                {
                    Code = "Opleidingen (department)",
                    Id = 101,
                    Name = "Opleidingen (department)"
                },
                Path = "/content/Hogeschool_Rotterdam/HR-FIT/HR-FIT-TAB"
            });

            IServiceProvider serviceProvider = CreateServiceProvider(mockedDatabase.Object, mockedApiClient.Object);
            ICommandDispatcher commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();

            // Act
            await commandDispatcher.DispatchAsync(new ProcessOrgUnits(batchSize: 1, isDepartmentType: true, isDeleteContext: false));

            // Assert
            mockedDatabase.VerifyAll();
            mockedApiClient.VerifyAll();
        }

        [TestMethod]
        public async Task GivenCustomOrgUnitToUpdateInDeleteContext_DoesNothing()
        {
            // Arrange
            mockedDatabase.Setup(database => database.GetNextCustomOrgUnitAsync(default)).ReturnsAsync(new OrgUnitRecord
            {
                Code = "HR-FIT",
                Name = "Dienst FIT",
                Parents = new[] { RootOrgId },
                SyncEventId = Random.Shared.Next(1, int.MaxValue),
                SyncAction = 'u',
                SyncExternalKey = Random.Shared.Next(1, int.MaxValue).ToString(),
                SyncInternalKey = Guid.NewGuid().ToString(),
                Type = 102
            });

            IServiceProvider serviceProvider = CreateServiceProvider(mockedDatabase.Object, mockedApiClient.Object);
            ICommandDispatcher commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();

            // Act
            await commandDispatcher.DispatchAsync(new ProcessOrgUnits(batchSize: 1, isDeleteContext: true));

            // Assert
            mockedDatabase.Verify(database => database.GetNextCustomOrgUnitAsync(It.IsAny<CancellationToken>()), Times.Once());
            mockedDatabase.VerifyNoOtherCalls();

            mockedApiClient.VerifyNoOtherCalls();
        }
    }
}
