using HR.BrightspaceConnector.Features.Users;
using HR.BrightspaceConnector.Infrastructure;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Linq;
using System.Text.Json;

namespace HR.BrightspaceConnector.Tests
{
    [TestClass]
    public class PagedResultSetTests
    {
        #region Json string data
        private string jsonString = @"{
            ""PagingInfo"": {
                ""Bookmark"": ""172"",
                ""HasMoreItems"": false
            },
            ""Items"": [
                {
                    ""OrgId"": 6606,
                    ""UserId"": 171,
                    ""FirstName"": ""D2L.Demo"",
                    ""MiddleName"": """",
                    ""LastName"": ""Instructor"",
                    ""UserName"": ""Demo.Instructor"",
                    ""ExternalEmail"": ""demo.instructor@d2l.com"",
                    ""OrgDefinedId"": ""Demo.Instructor"",
                    ""UniqueIdentifier"": ""Demo.Instructor"",
                    ""Activation"": {
                        ""IsActive"": true
                    },
                    ""DisplayName"": ""D2L.Demo Instructor"",
                    ""LastAccessedDate"": ""2020-09-30T19:36:30.873Z"",
                    ""Pronouns"": """"
                },
                {
                    ""OrgId"": 6606,
                    ""UserId"": 172,
                    ""FirstName"": ""D2L.Demo"",
                    ""MiddleName"": """",
                    ""LastName"": ""Student"",
                    ""UserName"": ""Demo.Student"",
                    ""ExternalEmail"": ""Demo.Student@d2l.com"",
                    ""OrgDefinedId"": ""Demo.Student"",
                    ""UniqueIdentifier"": ""Demo.Student"",
                    ""Activation"": {
                        ""IsActive"": true
                    },
                    ""DisplayName"": ""D2L.Demo Student"",
                    ""LastAccessedDate"": ""2019-02-21T21:54:32.123Z"",
                    ""Pronouns"": """"
                }
            ]
        }";
        #endregion

        private readonly JsonSerializerOptions jsonOptions = new(JsonSerializerDefaults.General);

        [TestMethod]
        public void DeserializeUsingCustomConverter()
        {
            var pagedResultSet = JsonSerializer.Deserialize<PagedResultSet<UserData>>(jsonString, jsonOptions);

            Assert.IsNotNull(pagedResultSet);
            Assert.AreEqual("172", pagedResultSet.PagingInfo.Bookmark);
            Assert.IsFalse(pagedResultSet.PagingInfo.HasMoreItems);
            Assert.AreEqual(2, pagedResultSet.Items.Count());
        }

        [TestMethod]
        public void SerializeUsingCustomConverter()
        {
            var pagedResultSet = new PagedResultSet<UserData>
            {
                Items = new[]
                {
                    new UserData { UserName = "Demo.Student", FirstName = "D2L.Demo", LastName = "Student" },
                    new UserData { UserName = "Demo.Instructor", FirstName = "D2L.Demo", LastName = "Instructor" }
                }
            };

            var jsonString = JsonSerializer.Serialize(pagedResultSet, jsonOptions);

            Assert.IsNotNull(jsonString);
            Assert.AreNotEqual(string.Empty, jsonString);
            Assert.IsTrue(jsonString.StartsWith("{") && jsonString.EndsWith("}"));
        }
    }
}
