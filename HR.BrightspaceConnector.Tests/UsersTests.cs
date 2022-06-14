using HR.BrightspaceConnector.Features.Users;
using HR.Common.Utilities;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Linq;
using System.Threading.Tasks;

namespace HR.BrightspaceConnector.Tests
{
    [TestClass]
    public class UsersTests : IntegrationTestsBase
    {
        [TestMethod]
        public async Task GetRolesAsync_ReturnsRoles()
        {
            IApiClient apiClient = CreateApiClient();

            var roles = await apiClient.GetRolesAsync().WithoutCapturingContext();
            Assert.IsTrue(roles.Any());

            roles = roles.Where(role => role.DisplayName == "Learner");
            Assert.IsTrue(roles.Any());
        }

        [TestMethod]
        public async Task CompleteLifecycleIntegrationTest()
        {
            IApiClient apiClient = CreateApiClient();

            var roles = await apiClient.GetRolesAsync().WithoutCapturingContext();
            var learnerRole = roles.Single(role => role.DisplayName == "Learner");

            var userToCreate = new CreateUserData
            {
                FirstName = "Jim",
                LastName = "Atas",
                OrgDefinedId = "ja.hstest@hro.nl",
                ExternalEmail = "ja.hstest@hr.nl",
                UserName = "ja.hstest",
                RoleId = learnerRole.Identifier,
                IsActive = true,
                SendCreationEmail = false
            };

            var createdUser = await apiClient.CreateUserAsync(userToCreate).WithoutCapturingContext();
            Assert.IsNotNull(createdUser.UserId);
            Assert.IsFalse(createdUser.UserId.IsNullOrDefault());

            var userToUpdate = new UpdateUserData
            {
                FirstName = "Jimbo",
                LastName = "Atas",
                OrgDefinedId = "ja.hstest@hro.nl",
                ExternalEmail = "ja.hstest@hr.nl",
                UserName = "ja.hstest",
                Activation = new UserActivationData { IsActive = true }
            };
            var updatedUser = await apiClient.UpdateUserAsync((int)createdUser.UserId, userToUpdate).WithoutCapturingContext();
            Assert.IsNotNull(updatedUser.UserId);
            Assert.IsFalse(updatedUser.UserId.IsNullOrDefault());
            Assert.AreEqual("Jimbo", updatedUser.FirstName);

            await apiClient.DeleteUserAsync((int)updatedUser.UserId).WithoutCapturingContext();
        }
    }
}
