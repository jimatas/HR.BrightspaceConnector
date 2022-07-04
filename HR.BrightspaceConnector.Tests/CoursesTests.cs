using HR.BrightspaceConnector.Features.Courses;
using HR.Common.Utilities;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Threading.Tasks;

namespace HR.BrightspaceConnector.Tests
{
    [TestClass]
    public class CoursesTests : IntegrationTestsBase
    {
        [TestMethod]
        public async Task CreateAndDeleteCourseTemplateAsync_UnderRootOrganization_CreatesAndDeletesCourseTemplate()
        {
            IApiClient apiClient = CreateApiClient();
            var rootOrganization = await apiClient.GetOrganizationAsync();

            var newCourseTemplate = await apiClient.CreateCourseTemplateAsync(new CreateCourseTemplate
            {
                Code = "HR-SampleCourseTemplate",
                Name = "Sample course template created by a unit test",
                ParentOrgUnitIds = new[] { (int)rootOrganization.Identifier! }
            });

            Assert.IsNotNull(newCourseTemplate);
            Assert.IsTrue(!newCourseTemplate.Identifier.IsNullOrDefault(), "!newCourseTemplate.Identifier.IsNullOrDefault()");

            await apiClient.DeleteCourseTemplateAsync((int)newCourseTemplate.Identifier!);
        }
    }
}
