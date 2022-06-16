using HR.BrightspaceConnector.Features.OrgUnits;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HR.BrightspaceConnector.Tests
{
    [TestClass]
    public class OrgUnitsTests : IntegrationTestsBase
    {
        [TestMethod]
        public async Task GetOrganizationAsync_ReturnsRootOrgUnit()
        {
            IApiClient apiClient = CreateApiClient();

            var organization = await apiClient.GetOrganizationAsync();

            Assert.IsNotNull(organization);
            Assert.AreEqual("Hogeschool Rotterdam", organization.Name);
        }

        [TestMethod]
        public async Task GetOrgUnitTypesAsync_ReturnsOrgUnitTypes()
        {
            IApiClient apiClient = CreateApiClient();

            var orgUnitTypes = await apiClient.GetOrgUnitTypes();

            Assert.IsNotNull(orgUnitTypes);
            Assert.IsTrue(orgUnitTypes.Any(), "orgUnitTypes.Any()");

            var departments = orgUnitTypes.Where(type => type.Name?.StartsWith("Instituten", StringComparison.OrdinalIgnoreCase) == true);
            Assert.IsTrue(departments.Any(), "departments.Any()");
        }

        [TestMethod]
        public async Task GetChildOrgUnitsAsync_ReturnsChildren()
        {
            IApiClient apiClient = CreateApiClient();

            var rootOrganization = await apiClient.GetOrganizationAsync();

            var children = await apiClient.GetChildOrgUnitsAsync((int)rootOrganization.Identifier!);
            Assert.IsTrue(children.Any(), "children.Any()");
        }

        [TestMethod]
        public async Task GetDescendantOrgUnitsAsync_ReturnsDescendants()
        {
            IApiClient apiClient = CreateApiClient();

            var rootOrganization = await apiClient.GetOrganizationAsync();
            
            var descendants = await apiClient.GetDescendantOrgUnitsAsync((int)rootOrganization.Identifier!);
            Assert.IsTrue(descendants.Any(), "descendants.Any()");
        }

        [TestMethod]
        public async Task CompleteLifecycleIntegrationTest()
        {
            IApiClient apiClient = CreateApiClient();

            Organization rootOrganization = await apiClient.GetOrganizationAsync();

            IEnumerable<OrgUnitType> orgUnitTypes = await apiClient.GetOrgUnitTypes();
            OrgUnitType departmentType = orgUnitTypes.Single(type => type.Name?.StartsWith("Instituten", StringComparison.OrdinalIgnoreCase) == true);

            var department = new OrgUnitCreateData
            {
                Code = "HR-FIT-Webcentrum",
                Name = "Dienst FIT/Webcentrum",
                Type = departmentType.Id,
                Parents = new[] { (int)rootOrganization.Identifier! }
            };

            OrgUnit newDepartment = await apiClient.CreateOrgUnitAsync(department);

            await apiClient.DeleteOrgUnitAsync((int)newDepartment.Identifier!, (int)rootOrganization.Identifier!);
        }
    }
}
