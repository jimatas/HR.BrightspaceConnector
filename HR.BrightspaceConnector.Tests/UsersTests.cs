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
        public async Task GetRolesAsync_ReturnsRole()
        {
            var apiClient = CreateApiClient();

            var roles = await apiClient.GetRolesAsync().WithoutCapturingContext();
            Assert.IsNotNull(roles);
            Assert.IsTrue(roles.Any());
        }
    }
}
