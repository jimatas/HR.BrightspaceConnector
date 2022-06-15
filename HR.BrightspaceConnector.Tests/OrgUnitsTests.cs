using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Threading.Tasks;

namespace HR.BrightspaceConnector.Tests
{
    [TestClass]
    public class OrgUnitsTests : IntegrationTestsBase
    {
        [TestMethod]
        public async Task GetOrganizationAsync_ReturnsRootOrgUnit()
        {
            var apiClient = CreateApiClient();

            var organization = await apiClient.GetOrganizationAsync();

            Assert.IsNotNull(organization);
            Assert.AreEqual("Hogeschool Rotterdam", organization.Name);
        }
    }
}
