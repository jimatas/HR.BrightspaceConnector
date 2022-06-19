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
        private static readonly IDictionary<string, int> RoleIds = new Dictionary<string, int>()
        {
            { "Administrator", 117 },
            { "Instructor", 109 },
            { "Learner", 110 },
        };

        private readonly Mock<IDatabase> databaseMock = new();
        private readonly Mock<IApiClient> apiClientMock = new();

        [TestMethod]
        public async Task GivenUserToCreate_CreatesUser()
        {
            // Arrange
            const int eventId = 88001;

            var userRecord = new UserRecord
            {
                UserName = "ja.hstest",
                OrgDefinedId = "ja.hstest@hro.nl",
                RoleId = RoleIds["Administrator"],
                FirstName = "Jim",
                LastName = "Atas",
                MiddleName = null,
                SortLastName = "Atas",
                ExternalEmail = "ja.hstest@hr.nl",
                IsActive = true,
                SendCreationEmail = false,
                SyncEventId = eventId,
                SyncAction = 'c',
                SyncExternalKey = null,
                SyncInternalKey = "ja.hstest"
            };

            databaseMock.Setup(database => database.GetNextUserAsync(default)).ReturnsAsync(userRecord);
            databaseMock.Setup(database => database.MarkAsHandledAsync(It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<int>(), null, default));

            const int userId = 270;

            apiClientMock.Setup(apiClient => apiClient.CreateUserAsync(It.IsAny<CreateUserData>(), default)).ReturnsAsync(() => new UserData
            {
                Activation = new UserActivationData { IsActive = true },
                DisplayName = "Jim Atas",
                ExternalEmail = "ja.hstest@hr.nl",
                FirstName = "Jim",
                LastAccessedDate = null,
                LastName = "Atas",
                MiddleName = null,
                OrgDefinedId = "ja.hstest@hro.nl",
                OrgId = 6606,
                Pronouns = null,
                UniqueIdentifier = "ja.hstest",
                UserId = userId,
                UserName = "ja.hstest",
            });

            IServiceProvider serviceProvider = CreateServiceProvider(databaseMock.Object, apiClientMock.Object);
            ICommandDispatcher commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();

            // Act
            await commandDispatcher.DispatchAsync(new ProcessUsers(batchSize: 1, isDeleteContext: false));

            // Assert
            databaseMock.Verify(database => database.GetNextUserAsync(default), Times.Once());
            databaseMock.Verify(database => database.MarkAsHandledAsync(eventId, true, userId, null, default), Times.Once());

            apiClientMock.Verify(apiClient => apiClient.CreateUserAsync(It.Is<CreateUserData>(createUserData => createUserData.UserName!.Equals(userRecord.ToCreateUserData().UserName)), default), Times.Once());
            apiClientMock.Verify(apiClient => apiClient.UpdateLegalPreferredNamesAsync(userId, It.IsAny<LegalPreferredNames>(), default), Times.Never());
        }
    }
}
