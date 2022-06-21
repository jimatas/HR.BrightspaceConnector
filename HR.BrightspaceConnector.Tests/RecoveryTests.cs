using HR.BrightspaceConnector.Features.Common.Commands;
using HR.BrightspaceConnector.Features.Users;
using HR.BrightspaceConnector.Features.Users.Queries;
using HR.BrightspaceConnector.Infrastructure.Persistence;
using HR.BrightspaceConnector.Tests.Fixture;
using HR.Common.Cqrs.Commands;
using HR.Common.Cqrs.Queries;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace HR.BrightspaceConnector.Tests
{
    [TestClass]
    public class RecoveryTests : DependencyInjectedTestsBase
    {
        private readonly Mock<IDatabase> mockedDatabase = new();
        private readonly Mock<IApiClient> mockedApiClient = new();

        [TestMethod]
        public async Task GetNextUserQueryFailingOnFirstAttempt_WillBeRetried()
        {
            // Arrange
            int attempt = 0;
            mockedDatabase.Setup(database => database.GetNextUserAsync(default)).ReturnsAsync(new UserRecord
            {
                UserName = "ja.hstest",
                OrgDefinedId = "ja.hstest@hro.nl",
                RoleId = 117,
                FirstName = "Jim",
                LastName = "Atas",
                SortLastName = "Atas",
                ExternalEmail = "ja.hstest@hr.nl",
                IsActive = true,
                SendCreationEmail = false,
                SyncEventId = Random.Shared.Next(1, int.MaxValue),
                SyncAction = 'c',
                SyncInternalKey = "ja.hstest"
            }).Callback((CancellationToken _) =>
            {
                if (++attempt == 1)
                {
                    throw new SqlTimeoutException();
                }
            });

            IServiceProvider serviceProvider = CreateServiceProvider(mockedDatabase.Object, mockedApiClient.Object);
            IQueryDispatcher queryDispatcher = serviceProvider.GetRequiredService<IQueryDispatcher>();

            // Act / Assert
            try
            {
                var user = await queryDispatcher.DispatchAsync(new GetNextUser());
                Assert.IsNotNull(user);
            }
            catch (DbException)
            {
                Assert.Fail("The exception should have been caught and the query retried.");
            }
        }

        [TestMethod]
        public async Task MarkAsHandledCommandFailingOnFirstAttempt_WillBeRetried()
        {
            // Arrange
            int attempt = 0;
            mockedDatabase.Setup(database => database.MarkAsHandledAsync(It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<int?>(), It.IsAny<string?>(), It.IsAny<CancellationToken>())).Callback((int _, bool _, int? _, string _, CancellationToken _) =>
            {
                if (++attempt == 1)
                {
                    throw new SqlTimeoutException();
                }
            });

            IServiceProvider serviceProvider = CreateServiceProvider(mockedDatabase.Object, mockedApiClient.Object);
            ICommandDispatcher commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();

            // Act / Assert
            try
            {
                await commandDispatcher.DispatchAsync(new MarkAsHandled(eventId: Random.Shared.Next(1, int.MaxValue), success: true, id: Random.Shared.Next(1, int.MaxValue), message: null));
            }
            catch (DbException)
            {
                Assert.Fail("The exception should have been caught and the command retried.");
            }
        }
    }
}
