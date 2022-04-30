using HR.BrightspaceConnector.Features.Users;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Text.Json;

namespace HR.BrightspaceConnector.Tests
{
    [TestClass]
    public class UserQueryParametersTests
    {
        [TestMethod]
        public void ToQueryString_NoPropertiesSet_ReturnsEmptyString()
        {
            // Arrange
            var queryParameters = new UserQueryParameters();

            // Act
            var queryString = queryParameters.ToQueryString();

            // Assert
            Assert.AreEqual(string.Empty, queryString);
        }

        [TestMethod]
        public void ToQueryString_ExternalEmailSet_ReturnsQueryString()
        {
            // Arrange
            var queryParameters = new UserQueryParameters
            {
                ExternalEmail = "test.user@hr.nl"
            };

            // Act
            var queryString = queryParameters.ToQueryString();

            // Assert
            Assert.IsTrue(queryString.StartsWith("?ExternalEmail="));
        }

        [TestMethod]
        public void ToQueryString_ExternalEmailSetAndPropertyNamingPolicyProvided_ReturnsQueryStringWithExpectedNaming()
        {
            // Arrange
            var queryParameters = new UserQueryParameters
            {
                ExternalEmail = "test.user@hr.nl"
            };

            // Act
            var queryString = queryParameters.ToQueryString(JsonNamingPolicy.CamelCase);

            // Assert
            Assert.IsTrue(queryString.StartsWith("?externalEmail="));
        }

        [TestMethod]
        public void ToQueryString_OrgDefinedIdSet_ReturnsQueryString()
        {
            // Arrange
            var queryParameters = new UserQueryParameters
            {
                OrgDefinedId = "test.user"
            };

            // Act
            var queryString = queryParameters.ToQueryString();

            // Assert
            Assert.AreEqual("?OrgDefinedId=test.user", queryString);
        }

        [TestMethod]
        public void ToQueryString_ExternalEmailSet_EscapesAtSign()
        {
            // Arrange
            var queryParameters = new UserQueryParameters
            {
                ExternalEmail = "test.user@hr.nl"
            };

            // Act
            var queryString = queryParameters.ToQueryString();

            // Assert
            Assert.AreEqual("?ExternalEmail=test.user%40hr.nl", queryString);
        }

        [TestMethod]
        public void ToQueryString_WithTwoPropertiesSet_AddsSeparator()
        {
            // Arrange
            var queryParameters = new UserQueryParameters
            {
                OrgDefinedId = "test.user",
                ExternalEmail = "test.user@hr.nl"
            };

            // Act
            var queryString = queryParameters.ToQueryString(JsonNamingPolicy.CamelCase);

            // Assert
            Assert.AreEqual("?orgDefinedId=test.user&externalEmail=test.user%40hr.nl", queryString);
        }
    }
}
