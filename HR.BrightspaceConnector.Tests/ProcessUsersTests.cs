using HR.BrightspaceConnector.Features.Users;
using HR.BrightspaceConnector.Features.Users.Commands;
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

        [TestMethod]
        public async Task GivenUserToCreate_CreatesUser()
        {
            // Arrange
            int eventId = Random.Shared.Next(1, int.MaxValue);

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
            mockedDatabase.Setup(database => database.MarkAsHandledAsync(It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<int>(), null, default));

            int userId = Random.Shared.Next(1, int.MaxValue);

            mockedApiClient.Setup(apiClient => apiClient.CreateUserAsync(It.IsAny<CreateUserData>(), default)).ReturnsAsync(() => new UserData
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
            mockedDatabase.Verify(database => database.GetNextUserAsync(default), Times.Once());
            mockedDatabase.Verify(database => database.MarkAsHandledAsync(eventId, true, userId, null, default), Times.Once());

            mockedApiClient.Verify(apiClient => apiClient.CreateUserAsync(It.Is<CreateUserData>(createUserData => Assert.That.AreEqual(userRecord.ToCreateUserData(), createUserData)), default), Times.Once());
            mockedApiClient.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task GivenUserToCreateWithDifferentSortLastName_CreatesUserAndUpdatesLegalAndPreferredNames()
        {
            // Arrange
            int eventId = Random.Shared.Next(1, int.MaxValue);

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
            mockedDatabase.Setup(database => database.MarkAsHandledAsync(It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<int>(), null, default));

            int userId = Random.Shared.Next(1, int.MaxValue);

            mockedApiClient.Setup(apiClient => apiClient.CreateUserAsync(It.IsAny<CreateUserData>(), default)).ReturnsAsync(() => new UserData
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
            mockedDatabase.Verify(database => database.GetNextUserAsync(default), Times.Once());
            mockedDatabase.Verify(database => database.MarkAsHandledAsync(eventId, true, userId, null, default), Times.Once());

            mockedApiClient.Verify(apiClient => apiClient.CreateUserAsync(It.Is<CreateUserData>(createUserData => Assert.That.AreEqual(userRecord.ToCreateUserData(), createUserData)), default), Times.Once());
            mockedApiClient.Verify(apiClient => apiClient.UpdateLegalPreferredNamesAsync(userId, It.Is<LegalPreferredNames>(legalPreferredNames => Assert.That.AreEqual(userRecord.ToLegalPreferredNames(), legalPreferredNames)), default), Times.Once());
        }

        [TestMethod]
        public async Task GivenUserToCreateInDeleteContext_DoesNothing()
        {
            // Arrange
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
                SyncEventId = Random.Shared.Next(1, int.MaxValue),
                SyncAction = 'c',
                SyncInternalKey = "ja.hstest"
            };

            mockedDatabase.Setup(database => database.GetNextUserAsync(default)).ReturnsAsync(userRecord);

            IServiceProvider serviceProvider = CreateServiceProvider(mockedDatabase.Object, mockedApiClient.Object);
            ICommandDispatcher commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();

            // Act
            await commandDispatcher.DispatchAsync(new ProcessUsers(batchSize: 1, isDeleteContext: true));

            // Assert
            mockedDatabase.Verify(database => database.GetNextUserAsync(default), Times.Once());
            mockedDatabase.VerifyNoOtherCalls();

            mockedApiClient.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task GivenUserToDelete_DeletesUser()
        {
            // Arrange
            var eventId = Random.Shared.Next(1, int.MaxValue);
            var userId = Random.Shared.Next(1, int.MaxValue);

            var userRecord = new UserRecord
            {
                SyncEventId = eventId,
                SyncAction = 'd',
                SyncExternalKey = userId.ToString(),
                SyncInternalKey = "ja.hstest"
            };

            mockedDatabase.Setup(database => database.GetNextUserAsync(default)).ReturnsAsync(userRecord);
            mockedDatabase.Setup(database => database.MarkAsHandledAsync(It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<int>(), null, default));

            mockedApiClient.Setup(apiClient => apiClient.DeleteUserAsync(It.IsAny<int>(), default));

            IServiceProvider serviceProvider = CreateServiceProvider(mockedDatabase.Object, mockedApiClient.Object);
            ICommandDispatcher commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();

            // Act
            await commandDispatcher.DispatchAsync(new ProcessUsers(batchSize: 1, isDeleteContext: true));

            // Assert
            mockedDatabase.Verify(database => database.GetNextUserAsync(default), Times.Once());
            mockedDatabase.Verify(database => database.MarkAsHandledAsync(eventId, true, userId, null, default), Times.Once());

            mockedApiClient.Verify(apiClient => apiClient.DeleteUserAsync(userId, default), Times.Once());
        }

        [TestMethod]
        public async Task GivenUserToDeleteInNonDeleteContext_DoesNothing()
        {
            // Arrange
            var userRecord = new UserRecord
            {
                SyncEventId = Random.Shared.Next(1, int.MaxValue),
                SyncAction = 'd',
                SyncExternalKey = Random.Shared.Next(1, int.MaxValue).ToString(),
                SyncInternalKey = "ja.hstest"
            };

            mockedDatabase.Setup(database => database.GetNextUserAsync(default)).ReturnsAsync(userRecord);
            mockedDatabase.Setup(database => database.MarkAsHandledAsync(It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<int>(), null, default));

            mockedApiClient.Setup(apiClient => apiClient.DeleteUserAsync(It.IsAny<int>(), default));

            IServiceProvider serviceProvider = CreateServiceProvider(mockedDatabase.Object, mockedApiClient.Object);
            ICommandDispatcher commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();

            // Act
            await commandDispatcher.DispatchAsync(new ProcessUsers(batchSize: 1, isDeleteContext: false));

            // Assert
            mockedDatabase.Verify(database => database.GetNextUserAsync(default), Times.Once());
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
            mockedDatabase.Setup(database => database.MarkAsHandledAsync(It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<int>(), null, default));

            mockedApiClient.Setup(apiClient => apiClient.UpdateUserAsync(It.IsAny<int>(), It.IsAny<UpdateUserData>(), default)).ReturnsAsync(() => new UserData
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
            mockedDatabase.Verify(database => database.GetNextUserAsync(default), Times.Once());
            mockedDatabase.Verify(database => database.MarkAsHandledAsync(eventId, true, userId, null, default), Times.Once());

            mockedApiClient.Verify(apiClient => apiClient.UpdateUserAsync(userId, It.Is<UpdateUserData>(updateUserData => Assert.That.AreEqual(userRecord.ToUpdateUserData(), updateUserData)), default), Times.Once());
            mockedApiClient.VerifyNoOtherCalls();
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
            mockedDatabase.Setup(database => database.MarkAsHandledAsync(It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<int>(), null, default));

            mockedApiClient.Setup(apiClient => apiClient.UpdateUserAsync(It.IsAny<int>(), It.IsAny<UpdateUserData>(), default)).ReturnsAsync(() => new UserData
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
            mockedDatabase.Verify(database => database.GetNextUserAsync(default), Times.Once());
            mockedDatabase.Verify(database => database.MarkAsHandledAsync(eventId, true, userId, null, default), Times.Once());

            mockedApiClient.Verify(apiClient => apiClient.UpdateUserAsync(userId, It.Is<UpdateUserData>(updateUserData => Assert.That.AreEqual(userRecord.ToUpdateUserData(), updateUserData)), default), Times.Once());
            mockedApiClient.Verify(apiClient => apiClient.UpdateLegalPreferredNamesAsync(userId, It.Is<LegalPreferredNames>(legalPreferredNames => Assert.That.AreEqual(userRecord.ToLegalPreferredNames(), legalPreferredNames)), default), Times.Once());
        }

        [TestMethod]
        public async Task GivenUserToUpdateInDeleteContext_DoesNothing()
        {
            // Arrange
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
                SyncEventId = Random.Shared.Next(1, int.MaxValue),
                SyncAction = 'u',
                SyncExternalKey = Random.Shared.Next(1, int.MaxValue).ToString(),
                SyncInternalKey = "ja.hstest"
            };

            mockedDatabase.Setup(database => database.GetNextUserAsync(default)).ReturnsAsync(userRecord);

            IServiceProvider serviceProvider = CreateServiceProvider(mockedDatabase.Object, mockedApiClient.Object);
            ICommandDispatcher commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();

            // Act
            await commandDispatcher.DispatchAsync(new ProcessUsers(batchSize: 1, isDeleteContext: true));

            // Assert
            mockedDatabase.Verify(database => database.GetNextUserAsync(default), Times.Once());
            mockedDatabase.VerifyNoOtherCalls();

            mockedApiClient.VerifyNoOtherCalls();
        }
    }
}
