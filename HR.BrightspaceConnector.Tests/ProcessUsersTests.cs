using HR.BrightspaceConnector.Features.Users;
using HR.BrightspaceConnector.Features.Users.Commands;
using HR.BrightspaceConnector.Infrastructure.Persistence;
using HR.Common.Cqrs.Commands;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HR.BrightspaceConnector.Tests
{
    [TestClass]
    public class ProcessUsersTests : DependencyInjectedTestsBase
    {
        private const int OrgId = 6606;
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
        public async Task GivenNullUserRegardlessOfContext_DoesNothing(bool isDeleteContext)
        {
            // Arrange
            mockedDatabase.Setup(database => database.GetNextUserAsync(default)).ReturnsAsync(null as UserRecord);

            IServiceProvider serviceProvider = CreateServiceProvider(mockedDatabase.Object, mockedApiClient.Object);
            ICommandDispatcher commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();

            // Act
            await commandDispatcher.DispatchAsync(new ProcessUsers(batchSize: 1, isDeleteContext));

            // Assert
            mockedDatabase.Verify(database => database.GetNextUserAsync(It.IsAny<CancellationToken>()), Times.Once());
            mockedDatabase.VerifyNoOtherCalls();

            mockedApiClient.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task GivenUserToCreate_CreatesUser()
        {
            // Arrange
            int eventId = Random.Shared.Next(1, int.MaxValue);
            int userId = Random.Shared.Next(1, int.MaxValue);

            var userRecord = new UserRecord
            {
                UserName = "ja.hstest",
                OrgDefinedId = "ja.hstest@hro.nl",
                RoleId = RoleIds["Administrator"],
                FirstName = "Jim",
                LastName = "Atas",
                SortLastName = "Atas",
                ExternalEmail = "ja.hstest@hr.nl",
                IsActive = true,
                SendCreationEmail = false,
                SyncEventId = eventId,
                SyncAction = 'c',
                SyncInternalKey = "ja.hstest"
            };

            mockedDatabase.Setup(database => database.GetNextUserAsync(default)).ReturnsAsync(userRecord);
            mockedDatabase.Setup(database => database.MarkAsHandledAsync(eventId, true, userId.ToString(), null, default));

            mockedApiClient.Setup(apiClient => apiClient.CreateUserAsync(It.Is<CreateUserData>(createUserData => Assert.That.AreEqual(userRecord.ToCreateUserData(), createUserData)), default)).ReturnsAsync(() => new UserData
            {
                Activation = new UserActivationData { IsActive = true },
                DisplayName = "Jim Atas",
                ExternalEmail = "ja.hstest@hr.nl",
                FirstName = "Jim",
                LastName = "Atas",
                OrgDefinedId = "ja.hstest@hro.nl",
                OrgId = OrgId,
                UniqueIdentifier = "ja.hstest",
                UserId = userId,
                UserName = "ja.hstest",
            });

            IServiceProvider serviceProvider = CreateServiceProvider(mockedDatabase.Object, mockedApiClient.Object);
            ICommandDispatcher commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();

            // Act
            await commandDispatcher.DispatchAsync(new ProcessUsers(batchSize: 1, isDeleteContext: false));

            // Assert
            mockedDatabase.VerifyAll();

            mockedApiClient.VerifyAll();
            mockedApiClient.Verify(apiClient => apiClient.UpdateLegalPreferredNamesAsync(It.IsAny<int>(), It.IsAny<LegalPreferredNames>(), It.IsAny<CancellationToken>()), Times.Never());
        }

        [TestMethod]
        public async Task GivenUserToCreateWithDifferentSortLastName_CreatesUserAndUpdatesLegalAndPreferredNames()
        {
            // Arrange
            int eventId = Random.Shared.Next(1, int.MaxValue);
            int userId = Random.Shared.Next(1, int.MaxValue);

            var userRecord = new UserRecord
            {
                UserName = "ja.hstest",
                OrgDefinedId = "ja.hstest@hro.nl",
                RoleId = RoleIds["Administrator"],
                FirstName = "Jim",
                LastName = "Atas",
                SortLastName = "Ätas", // Note: Accented "A".
                ExternalEmail = "ja.hstest@hr.nl",
                IsActive = true,
                SendCreationEmail = false,
                SyncEventId = eventId,
                SyncAction = 'c',
                SyncInternalKey = "ja.hstest"
            };

            mockedDatabase.Setup(database => database.GetNextUserAsync(default)).ReturnsAsync(userRecord);
            mockedDatabase.Setup(database => database.MarkAsHandledAsync(eventId, true, userId.ToString(), null, default));

            mockedApiClient.Setup(apiClient => apiClient.CreateUserAsync(It.Is<CreateUserData>(createUserData => Assert.That.AreEqual(userRecord.ToCreateUserData(), createUserData)), default)).ReturnsAsync(() => new UserData
            {
                Activation = new UserActivationData { IsActive = true },
                DisplayName = "Jim Atas",
                ExternalEmail = "ja.hstest@hr.nl",
                FirstName = "Jim",
                LastName = "Atas",
                OrgDefinedId = "ja.hstest@hro.nl",
                OrgId = OrgId,
                UniqueIdentifier = "ja.hstest",
                UserId = userId,
                UserName = "ja.hstest",
            });

            mockedApiClient.Setup(apiClient => apiClient.UpdateLegalPreferredNamesAsync(userId, It.Is<LegalPreferredNames>(legalPreferredNames => Assert.That.AreEqual(userRecord.ToLegalPreferredNames(), legalPreferredNames)), default));

            IServiceProvider serviceProvider = CreateServiceProvider(mockedDatabase.Object, mockedApiClient.Object);
            ICommandDispatcher commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();

            // Act
            await commandDispatcher.DispatchAsync(new ProcessUsers(batchSize: 1, isDeleteContext: false));

            // Assert
            mockedDatabase.VerifyAll();
            mockedApiClient.VerifyAll();
        }

        [TestMethod]
        public async Task GivenUserToCreateInDeleteContext_DoesNothing()
        {
            // Arrange
            mockedDatabase.Setup(database => database.GetNextUserAsync(default)).ReturnsAsync(new UserRecord
            {
                UserName = "ja.hstest",
                OrgDefinedId = "ja.hstest@hro.nl",
                RoleId = RoleIds["Administrator"],
                FirstName = "Jim",
                LastName = "Atas",
                SortLastName = "Atas",
                ExternalEmail = "ja.hstest@hr.nl",
                IsActive = true,
                SendCreationEmail = false,
                SyncEventId = Random.Shared.Next(1, int.MaxValue),
                SyncAction = 'c',
                SyncInternalKey = "ja.hstest"
            });

            IServiceProvider serviceProvider = CreateServiceProvider(mockedDatabase.Object, mockedApiClient.Object);
            ICommandDispatcher commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();

            // Act
            await commandDispatcher.DispatchAsync(new ProcessUsers(batchSize: 1, isDeleteContext: true));

            // Assert
            mockedDatabase.Verify(database => database.GetNextUserAsync(It.IsAny<CancellationToken>()), Times.Once());
            mockedDatabase.VerifyNoOtherCalls();

            mockedApiClient.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task GivenUserToDelete_DeletesUser()
        {
            // Arrange
            var eventId = Random.Shared.Next(1, int.MaxValue);
            var userId = Random.Shared.Next(1, int.MaxValue);

            mockedDatabase.Setup(database => database.GetNextUserAsync(default)).ReturnsAsync(new UserRecord
            {
                SyncEventId = eventId,
                SyncAction = 'd',
                SyncExternalKey = userId.ToString(),
                SyncInternalKey = "ja.hstest"
            });
            mockedDatabase.Setup(database => database.MarkAsHandledAsync(eventId, true, userId.ToString(), null, default));

            mockedApiClient.Setup(apiClient => apiClient.DeleteUserAsync(userId, default));

            IServiceProvider serviceProvider = CreateServiceProvider(mockedDatabase.Object, mockedApiClient.Object);
            ICommandDispatcher commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();

            // Act
            await commandDispatcher.DispatchAsync(new ProcessUsers(batchSize: 1, isDeleteContext: true));

            // Assert
            mockedDatabase.VerifyAll();
            mockedApiClient.VerifyAll();
        }

        [TestMethod]
        public async Task GivenUserToDeleteInNonDeleteContext_DoesNothing()
        {
            // Arrange
            var eventId = Random.Shared.Next(1, int.MaxValue);
            var userId = Random.Shared.Next(1, int.MaxValue);

            mockedDatabase.Setup(database => database.GetNextUserAsync(default)).ReturnsAsync(new UserRecord
            {
                SyncEventId = eventId,
                SyncAction = 'd',
                SyncExternalKey = userId.ToString(),
                SyncInternalKey = "ja.hstest"
            });
            mockedDatabase.Setup(database => database.MarkAsHandledAsync(eventId, true, userId.ToString(), null, default));

            mockedApiClient.Setup(apiClient => apiClient.DeleteUserAsync(userId, default));

            IServiceProvider serviceProvider = CreateServiceProvider(mockedDatabase.Object, mockedApiClient.Object);
            ICommandDispatcher commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();

            // Act
            await commandDispatcher.DispatchAsync(new ProcessUsers(batchSize: 1, isDeleteContext: false));

            // Assert
            mockedDatabase.Verify(database => database.GetNextUserAsync(It.IsAny<CancellationToken>()), Times.Once());
            mockedDatabase.VerifyNoOtherCalls();

            mockedApiClient.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task GivenUserToUpdate_UpdatesUser()
        {
            // Arrange
            var eventId = Random.Shared.Next(1, int.MaxValue);
            var userId = Random.Shared.Next(1, int.MaxValue);

            var userRecord = new UserRecord
            {
                UserName = "ja.hstest",
                OrgDefinedId = "ja.hstest@hro.nl",
                RoleId = RoleIds["Administrator"],
                FirstName = "Jim",
                LastName = "Atas",
                SortLastName = "Atas",
                ExternalEmail = "ja.hstest@hr.nl",
                IsActive = true,
                SendCreationEmail = false,
                SyncEventId = eventId,
                SyncAction = 'u',
                SyncExternalKey = userId.ToString(),
                SyncInternalKey = "ja.hstest"
            };

            mockedDatabase.Setup(database => database.GetNextUserAsync(default)).ReturnsAsync(userRecord);
            mockedDatabase.Setup(database => database.MarkAsHandledAsync(eventId, true, userId.ToString(), null, default));

            mockedApiClient.Setup(apiClient => apiClient.UpdateUserAsync(userId, It.Is<UpdateUserData>(updateUserData => Assert.That.AreEqual(userRecord.ToUpdateUserData(), updateUserData)), default)).ReturnsAsync(() => new UserData
            {
                Activation = new UserActivationData { IsActive = true },
                DisplayName = "Jim Atas",
                ExternalEmail = "ja.hstest@hr.nl",
                FirstName = "Jim",
                LastName = "Atas",
                OrgDefinedId = "ja.hstest@hro.nl",
                OrgId = OrgId,
                UniqueIdentifier = "ja.hstest",
                UserId = userId,
                UserName = "ja.hstest",
            });

            IServiceProvider serviceProvider = CreateServiceProvider(mockedDatabase.Object, mockedApiClient.Object);
            ICommandDispatcher commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();

            // Act
            await commandDispatcher.DispatchAsync(new ProcessUsers(batchSize: 1, isDeleteContext: false));

            // Assert
            mockedDatabase.VerifyAll();

            mockedApiClient.VerifyAll();
            mockedApiClient.Verify(apiClient => apiClient.UpdateLegalPreferredNamesAsync(It.IsAny<int>(), It.IsAny<LegalPreferredNames>(), It.IsAny<CancellationToken>()), Times.Never());
        }

        [TestMethod]
        public async Task GivenUserToUpdateWithDifferentSortLastName_UpdatesUserAndUpdatesLegalAndPreferredNames()
        {
            // Arrange
            var eventId = Random.Shared.Next(1, int.MaxValue);
            var userId = Random.Shared.Next(1, int.MaxValue);

            var userRecord = new UserRecord
            {
                UserName = "ja.hstest",
                OrgDefinedId = "ja.hstest@hro.nl",
                RoleId = RoleIds["Administrator"],
                FirstName = "Jim",
                LastName = "Atas",
                SortLastName = "Ätas", // Note: Accented "A".
                ExternalEmail = "ja.hstest@hr.nl",
                IsActive = true,
                SendCreationEmail = false,
                SyncEventId = eventId,
                SyncAction = 'u',
                SyncExternalKey = userId.ToString(),
                SyncInternalKey = "ja.hstest"
            };

            mockedDatabase.Setup(database => database.GetNextUserAsync(default)).ReturnsAsync(userRecord);
            mockedDatabase.Setup(database => database.MarkAsHandledAsync(eventId, true, userId.ToString(), null, default));

            mockedApiClient.Setup(apiClient => apiClient.UpdateUserAsync(userId, It.Is<UpdateUserData>(updateUserData => Assert.That.AreEqual(userRecord.ToUpdateUserData(), updateUserData)), default)).ReturnsAsync(() => new UserData
            {
                Activation = new UserActivationData { IsActive = true },
                DisplayName = "Jim Atas",
                ExternalEmail = "ja.hstest@hr.nl",
                FirstName = "Jim",
                LastName = "Atas",
                OrgDefinedId = "ja.hstest@hro.nl",
                OrgId = OrgId,
                UniqueIdentifier = "ja.hstest",
                UserId = userId,
                UserName = "ja.hstest",
            });

            mockedApiClient.Setup(apiClient => apiClient.UpdateLegalPreferredNamesAsync(userId, It.Is<LegalPreferredNames>(legalPreferredNames => Assert.That.AreEqual(userRecord.ToLegalPreferredNames(), legalPreferredNames)), default));

            IServiceProvider serviceProvider = CreateServiceProvider(mockedDatabase.Object, mockedApiClient.Object);
            ICommandDispatcher commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();

            // Act
            await commandDispatcher.DispatchAsync(new ProcessUsers(batchSize: 1, isDeleteContext: false));

            // Assert
            mockedDatabase.VerifyAll();
            mockedApiClient.VerifyAll();
        }

        [TestMethod]
        public async Task GivenUserToUpdateInDeleteContext_DoesNothing()
        {
            // Arrange
            mockedDatabase.Setup(database => database.GetNextUserAsync(default)).ReturnsAsync(new UserRecord
            {
                UserName = "ja.hstest",
                OrgDefinedId = "ja.hstest@hro.nl",
                RoleId = RoleIds["Administrator"],
                FirstName = "Jim",
                LastName = "Atas",
                SortLastName = "Atas",
                ExternalEmail = "ja.hstest@hr.nl",
                IsActive = true,
                SendCreationEmail = false,
                SyncEventId = Random.Shared.Next(1, int.MaxValue),
                SyncAction = 'u',
                SyncExternalKey = Random.Shared.Next(1, int.MaxValue).ToString(),
                SyncInternalKey = "ja.hstest"
            });

            IServiceProvider serviceProvider = CreateServiceProvider(mockedDatabase.Object, mockedApiClient.Object);
            ICommandDispatcher commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();

            // Act
            await commandDispatcher.DispatchAsync(new ProcessUsers(batchSize: 1, isDeleteContext: true));

            // Assert
            mockedDatabase.Verify(database => database.GetNextUserAsync(It.IsAny<CancellationToken>()), Times.Once());
            mockedDatabase.VerifyNoOtherCalls();

            mockedApiClient.VerifyNoOtherCalls();
        }
    }
}
