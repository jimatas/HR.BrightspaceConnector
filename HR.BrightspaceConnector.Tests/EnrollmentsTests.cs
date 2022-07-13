using HR.BrightspaceConnector.Features.Common;
using HR.BrightspaceConnector.Features.Courses;
using HR.BrightspaceConnector.Features.Enrollments;
using HR.BrightspaceConnector.Features.Users;
using HR.BrightspaceConnector.Infrastructure.Persistence;
using HR.BrightspaceConnector.Utilities;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Linq;
using System.Threading.Tasks;

namespace HR.BrightspaceConnector.Tests
{
    [TestClass]
    public class EnrollmentsTests : IntegrationTestsBase
    {
        [TestMethod]
        public async Task GetNextCourseOfferingEnrollmentAsync_ReturnsNextEnrollmentOrNull()
        {
            IDatabase database = CreateDatabase();

            EnrollmentRecord? enrollment = await database.GetNextCourseOfferingEnrollmentAsync();

            if (enrollment is not null) // Null is a perfectly valid return value.
            {
                Assert.IsNotNull(enrollment.SyncEventId);
            }
        }

        [TestMethod]
        public async Task CompleteEnrollmentLifecycleTest()
        {
            IApiClient apiClient = CreateApiClient();
            var rootOrganization = await apiClient.GetOrganizationAsync();

            var newCourseTemplate = await apiClient.CreateCourseTemplateAsync(new CreateCourseTemplate
            {
                Code = "HR-SampleCourseTemplate",
                Name = "Sample course template created by a unit test",
                ParentOrgUnitIds = new[] { (int)rootOrganization.Identifier! }
            });

            var newCourseOffering = await apiClient.CreateCourseOfferingAsync(new CreateCourseOffering
            {
                Code = "HR-SampleCourseOffering",
                Name = "Sample course offering created by a unit test",
                Description = new RichTextInput
                {
                    Type = TextContentType.Text,
                    Content = "This course offering is a sample that was created by a unit test. You can safely ignore it."
                },
                CanSelfRegister = true,
                StartDate = SystemClock.Instance.Now,
                EndDate = SystemClock.Instance.Now.AddDays(31),
                CourseTemplateId = newCourseTemplate.Identifier,
                ForceLocale = true,
                IsActive = false,
                ShowAddressBook = true
            });

            var allRoles = await apiClient.GetRolesAsync();
            var learnerRole = allRoles.Single(role => role.DisplayName == "Learner");
            
            var newUser = await apiClient.CreateUserAsync(new CreateUserData
            {
                FirstName = "Jim",
                LastName = "Atas",
                OrgDefinedId = "ja.hstest@hro.nl",
                ExternalEmail = "ja.hstest@hr.nl",
                UserName = "ja.hstest",
                RoleId = learnerRole.Identifier,
                IsActive = true,
                SendCreationEmail = false
            });

            var newEnrollment = await apiClient.CreateOrUpdateEnrollmentAsync(new CreateEnrollmentData
            {
                OrgUnitId = newCourseOffering.Identifier,
                RoleId = learnerRole.Identifier,
                UserId = newUser.UserId
            });

            var instructorRole = allRoles.Single(role => role.DisplayName == "Instructor");

            var updatedEnrollment = await apiClient.CreateOrUpdateEnrollmentAsync(new CreateEnrollmentData
            {
                OrgUnitId = newCourseOffering.Identifier,
                RoleId = instructorRole.Identifier,
                UserId = newUser.UserId
            });

            var oldEnrollment = await apiClient.DeleteEnrollmentAsync((int)newUser.UserId!, (int)newCourseOffering.Identifier!);

            await apiClient.DeleteCourseOfferingAsync((int)newCourseOffering.Identifier, permanently: true);
            await apiClient.DeleteCourseTemplateAsync((int)newCourseTemplate.Identifier!, permanently: true);
            await apiClient.DeleteUserAsync((int)newUser.UserId);
        }
    }
}
