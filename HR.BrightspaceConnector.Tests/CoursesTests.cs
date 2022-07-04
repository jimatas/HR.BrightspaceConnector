using HR.BrightspaceConnector.Features.Courses;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Threading.Tasks;

namespace HR.BrightspaceConnector.Tests
{
    [TestClass]
    public class CoursesTests : IntegrationTestsBase
    {
        [TestMethod]
        public async Task CompleteCourseTemplateLifecycleIntegrationTest()
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

            var courseTemplateId = newCourseTemplate.Identifier;
            Assert.IsNotNull(courseTemplateId);

            await apiClient.UpdateCourseTemplateAsync((int)courseTemplateId, new CourseTemplateInfo
            {
                Code = "HR-SampleCourseTemplate-v2",
                Name = "Sample course template updated by a unit test"
            });

            var updatedCourseTemplate = await apiClient.GetCourseTemplateAsync((int)courseTemplateId);

            Assert.IsNotNull(updatedCourseTemplate);
            Assert.AreEqual("HR-SampleCourseTemplate-v2", updatedCourseTemplate.Code);
            Assert.AreEqual("Sample course template updated by a unit test", updatedCourseTemplate.Name);

            await apiClient.DeleteCourseTemplateAsync((int)courseTemplateId, permanently: true);
        }
    }
}
