using HR.BrightspaceConnector.Features.OrgUnits;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        public async Task CreateOrgUnitAsync_UnderRoot_CreatesOrgUnit()
        {
            IApiClient apiClient = CreateApiClient();

            Organization rootOrganization = await apiClient.GetOrganizationAsync();

            IEnumerable<OrgUnitType> orgUnitTypes = await apiClient.GetOrgUnitTypes();
            OrgUnitType orgUnitType = orgUnitTypes.Single(type => type.Name?.StartsWith("Instituten", StringComparison.OrdinalIgnoreCase) == true);

            var orgUnitToCreate = new OrgUnitCreateData
            {
                Code = "HR-FIT",
                Name = "Dienst FIT",
                Type = orgUnitType.Id,
                Parents = new[] { (int)rootOrganization.Identifier! }
            };

            OrgUnit newOrgUnit = await apiClient.CreateOrgUnitAsync(orgUnitToCreate);
            Assert.IsNotNull(newOrgUnit.Identifier);
            Assert.AreEqual(orgUnitToCreate.Code, newOrgUnit.Code);

            await apiClient.DeleteOrgUnitAsync((int)newOrgUnit.Identifier!, permanently: true);
        }

        [TestMethod]
        public async Task UpdateOrgUnitAsync_UpdatesOrgUnitProperties()
        {
            IApiClient apiClient = CreateApiClient();

            Organization rootOrganization = await apiClient.GetOrganizationAsync();

            IEnumerable<OrgUnitType> orgUnitTypes = await apiClient.GetOrgUnitTypes();
            OrgUnitType orgUnitType = orgUnitTypes.Single(type => type.Name?.StartsWith("Instituten", StringComparison.OrdinalIgnoreCase) == true);
            var orgUnitToCreate = new OrgUnitCreateData
            {
                Code = "HR-FIT",
                Name = "Dienst FIT",
                Type = orgUnitType.Id,
                Parents = new[] { (int)rootOrganization.Identifier! }
            };

            var newOrgUnit = await apiClient.CreateOrgUnitAsync(orgUnitToCreate);

            var orgUnitToUpdate = new OrgUnitProperties
            {
                Code = "HR-FIT",
                Name = "Dienst Faciliteiten en Informatietechnologie",
                Path = "/content/Hogeschool_Rotterdam/HR-FIT",
            };

            var updatedOrgUnit = await apiClient.UpdateOrgUnitAsync((int)newOrgUnit.Identifier!, orgUnitToUpdate);

            Assert.IsNotNull(updatedOrgUnit);
            Assert.AreEqual(newOrgUnit.Identifier, updatedOrgUnit.Identifier);
            Assert.AreNotEqual(newOrgUnit.Name, updatedOrgUnit.Name);
            Assert.IsFalse(string.IsNullOrEmpty(updatedOrgUnit.Path), "string.IsNullOrEmpty(updatedOrgUnit.Path)");

            await apiClient.DeleteOrgUnitAsync((int)newOrgUnit.Identifier, permanently: true);
        }

        [TestMethod]
        public async Task DeleteOrgUnitAsync_Permanently_RecyclesOrgUnit()
        {
            IApiClient apiClient = CreateApiClient();

            Organization rootOrganization = await apiClient.GetOrganizationAsync();

            IEnumerable<OrgUnitType> orgUnitTypes = await apiClient.GetOrgUnitTypes();
            OrgUnitType orgUnitType = orgUnitTypes.Single(type => type.Name?.StartsWith("Instituten", StringComparison.OrdinalIgnoreCase) == true);
            var orgUnitToCreate = new OrgUnitCreateData
            {
                Code = "HR-FIT",
                Name = "Dienst FIT",
                Type = orgUnitType.Id,
                Parents = new[] { (int)rootOrganization.Identifier! }
            };

            OrgUnit newOrgUnit = await apiClient.CreateOrgUnitAsync(orgUnitToCreate);
            Assert.IsNotNull(newOrgUnit.Identifier);
            Assert.AreEqual(orgUnitToCreate.Code, newOrgUnit.Code);

            await apiClient.DeleteOrgUnitAsync((int)newOrgUnit.Identifier, permanently: true);
        }

        [TestMethod]
        public async Task DeleteOrgUnitAsync_GivenOrgUnitWithChild_ThrowsException()
        {
            IApiClient apiClient = CreateApiClient();

            Organization rootOrganization = await apiClient.GetOrganizationAsync();

            IEnumerable<OrgUnitType> orgUnitTypes = await apiClient.GetOrgUnitTypes();
            OrgUnitType orgUnitType = orgUnitTypes.Single(type => type.Name?.StartsWith("Instituten", StringComparison.OrdinalIgnoreCase) == true);
            var orgUnitToCreate = new OrgUnitCreateData
            {
                Code = "HR-FIT",
                Name = "Dienst FIT",
                Type = orgUnitType.Id,
                Parents = new[] { (int)rootOrganization.Identifier! }
            };

            OrgUnit topLevelOrgUnit = await apiClient.CreateOrgUnitAsync(orgUnitToCreate);
            Assert.IsNotNull(topLevelOrgUnit.Identifier);
            Assert.AreEqual(orgUnitToCreate.Code, topLevelOrgUnit.Code);

            orgUnitType = orgUnitTypes.Single(type => type.Name?.StartsWith("Opleidingen", StringComparison.OrdinalIgnoreCase) == true);
            orgUnitToCreate = new OrgUnitCreateData
            {
                Code = "HR-FIT-TAB",
                Name = "Afdeling applicatiebeheer",
                Type = orgUnitType.Id,
                Parents = new[] { (int)topLevelOrgUnit.Identifier }
            };

            OrgUnit childOrgUnit = await apiClient.CreateOrgUnitAsync(orgUnitToCreate);
            Assert.IsNotNull(childOrgUnit.Identifier);
            Assert.AreEqual(orgUnitToCreate.Code, childOrgUnit.Code);

            Task action() => apiClient.DeleteOrgUnitAsync((int)topLevelOrgUnit.Identifier!);
            var exception = await Assert.ThrowsExceptionAsync<ApiException>(action);
            Assert.AreEqual(HttpStatusCode.BadRequest, exception.StatusCode);

            // Clean-up
            await apiClient.DeleteOrgUnitAsync((int)childOrgUnit.Identifier, permanently: true);
            await apiClient.DeleteOrgUnitAsync((int)topLevelOrgUnit.Identifier!, permanently: true);
        }
    }
}
