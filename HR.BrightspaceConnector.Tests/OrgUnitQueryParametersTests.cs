using HR.BrightspaceConnector.Features.OrgUnits;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Text.Json;

namespace HR.BrightspaceConnector.Tests
{
    [TestClass]
    public class OrgUnitQueryParametersTests
    {
        [TestMethod]
        public void ToQueryString_NoPropertiesSet_ReturnsEmptyString()
        {
            // Arrange
            OrgUnitQueryParameters queryParameters = new();

            // Act
            var queryString = queryParameters.ToQueryString();

            // Assert
            Assert.AreEqual(string.Empty, queryString);
        }

        [TestMethod]
        public void ToQueryString_OnePropertySet_ReturnsQueryStringWithThatProperty()
        {
            // Arrange
            var queryParameters = new OrgUnitQueryParameters
            {
                OrgUnitCode = "HR-FIT"
            };

            // Act
            var queryString = queryParameters.ToQueryString();

            // Assert
            Assert.AreEqual("?OrgUnitCode=HR-FIT", queryString);
        }

        [TestMethod]
        public void ToQueryString_TwoPropertiesSet_ReturnsQueryStringWithThoseProperties()
        {
            // Arrange
            var queryParameters = new OrgUnitQueryParameters
            {
                OrgUnitCode = "HR-FIT",
                OrgUnitType = 3
            };

            // Act
            var queryString = queryParameters.ToQueryString();

            // Assert
            Assert.AreEqual("?OrgUnitType=3&OrgUnitCode=HR-FIT", queryString);
        }

        [TestMethod]
        public void ToQueryString_OnPropertySetAndPropertyNamingPolicyProvided_ReturnsQueryStringWithExpectedNaming()
        {
            // Arrange
            var queryParameters = new OrgUnitQueryParameters
            {
                ExactOrgUnitCode = "HR-FIT"
            };

            // Act
            var queryString = queryParameters.ToQueryString(propertyNamingPolicy: JsonNamingPolicy.CamelCase);

            // Assert
            Assert.AreEqual("?exactOrgUnitCode=HR-FIT", queryString);
        }
    }
}
