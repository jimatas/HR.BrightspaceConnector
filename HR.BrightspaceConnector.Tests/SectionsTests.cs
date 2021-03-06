using HR.BrightspaceConnector.Features.Common;
using HR.BrightspaceConnector.Features.Courses;
using HR.BrightspaceConnector.Features.Enrollments;
using HR.BrightspaceConnector.Features.Sections;
using HR.BrightspaceConnector.Infrastructure.Persistence;
using HR.BrightspaceConnector.Utilities;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace HR.BrightspaceConnector.Tests
{
    [TestClass]
    public class SectionsTests : IntegrationTestsBase
    {
        [TestMethod]
        public async Task GetNextCourseOfferingSection_ReturnsSectionRecordOrNull()
        {
            IDatabase database = CreateDatabase();

            SectionRecord? section = await database.GetNextCourseOfferingSectionAsync();
            if (section is not null)
            {
                Assert.IsNotNull(section.SyncEventId);
                Assert.IsNotNull(section.SyncInternalKey);
            }
        }

        [TestMethod]
        public async Task CreateSectionSettings_WithPropertiesMissing_ThrowsException()
        {
            IApiClient apiClient = CreateApiClient();

            CourseOffering courseOffering = await CreateCourseOfferingAsync(apiClient);
            try
            {
                var newSectionSettings = await apiClient.CreateSectionSettingsAsync(
                    (int)courseOffering.Identifier!,
                    new CreateSectionSettingsData
                    {
                        AutoEnroll = false,
                        EnrollmentQuantity = null,
                        EnrollmentStyle = null, // Not specified
                        RandomizeEnrollments = false, // Not specified
                        DescriptionsVisibleToEnrollees = true
                    });
            }
            catch (ApiException exception)
            {
                Assert.AreEqual(HttpStatusCode.BadRequest, exception.StatusCode);
            }
            finally
            {
                await DeleteCourseOfferingAsync(apiClient, courseOffering);
            }
        }

        [TestMethod]
        public async Task CompleteLifecycleIntegrationTest()
        {
            var apiClient = CreateApiClient();

            CourseOffering courseOffering = await CreateCourseOfferingAsync(apiClient);
            Assert.IsNotNull(courseOffering.Identifier);

            try
            {
                try
                {
                    // Will throw 404 if there's no section settings defined for the course.
                    var sectionSettings = await apiClient.GetSectionSettingsAsync((int)courseOffering.Identifier);
                }
                catch (ApiException exception) when (exception.StatusCode == HttpStatusCode.NotFound)
                {
                    var newSectionSettings = await apiClient.CreateSectionSettingsAsync((int)courseOffering.Identifier,
                        new CreateSectionSettingsData
                        {
                            AutoEnroll = true,
                            DescriptionsVisibleToEnrollees = true,
                            EnrollmentQuantity = 3,
                            EnrollmentStyle = SectionEnrollmentStyle.NumberOfSectionsAutoEnrollment,
                            RandomizeEnrollments = true
                        });

                    // Will have the default name set.
                    Assert.AreEqual("Sections", newSectionSettings.Name);

                    var newerSectionSettings = await apiClient.UpdateSectionSettingsAsync((int)courseOffering.Identifier,
                        new UpdateSectionSettingsData
                        {
                            AutoEnroll = false,
                            DescriptionsVisibleToEnrollees = false,
                            RandomizeEnrollments = false,
                            Name = "Sample sections",
                            Description = new RichTextInput
                            {
                                Content = "These are sample sections created by a unit test.",
                                Type = TextContentType.Text
                            }
                        });

                    Assert.AreNotEqual(newSectionSettings.Name, newerSectionSettings.Name);
                }

                var newSection = await apiClient.CreateSectionAsync((int)courseOffering.Identifier,
                    new CreateOrUpdateSectionData
                    {
                        Code = "Section-4",
                        Name = "Sample section 4",
                        Description = new RichTextInput
                        {
                            Content = "This sample section was created by a unit test.",
                            Type = TextContentType.Text,
                        }
                    });

                var newerSection = await apiClient.UpdateSectionAsync((int)courseOffering.Identifier, (int)newSection.SectionId!,
                    new CreateOrUpdateSectionData
                    {
                        Code = "Sectie-4",
                        Name = "Voorbeeldsectie 4",
                        Description = new RichTextInput
                        {
                            Content = "Nieuwe sectie aangemaakt door een unit test.",
                            Type = TextContentType.Html
                        }
                    });

                var learnerRole = (await apiClient.GetRolesAsync()).Single(role => string.Equals(role.DisplayName, "Learner", StringComparison.OrdinalIgnoreCase));
                var newLearner = await apiClient.CreateUserAsync(
                    new Features.Users.CreateUserData
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

                await apiClient.CreateOrUpdateEnrollmentAsync(new CreateEnrollmentData
                {
                    OrgUnitId = courseOffering.Identifier,
                    UserId = newLearner.UserId,
                    RoleId = learnerRole.Identifier
                });

                await apiClient.CreateSectionEnrollmentAsync(
                    (int)courseOffering.Identifier,
                    (int)newerSection.SectionId!,
                    new SectionEnrollment { UserId = newLearner.UserId });

                var sections = await apiClient.GetSectionsAsync((int)courseOffering.Identifier);
                Assert.AreEqual(4, sections.Count());
                Assert.AreEqual(1, sections.Single(sec => sec.Code == "Sectie-4").Enrollments.Count());

                await apiClient.DeleteEnrollmentAsync((int)newLearner.UserId!, (int)courseOffering.Identifier);
                await apiClient.DeleteUserAsync((int)newLearner.UserId);

                foreach (var section in sections)
                {
                    await apiClient.DeleteSectionAsync((int)courseOffering.Identifier, (int)section.SectionId!);
                }
            }
            finally
            {
                await DeleteCourseOfferingAsync(apiClient, courseOffering);
            }
        }
        
        private static async Task<CourseOffering> CreateCourseOfferingAsync(IApiClient apiClient)
        {
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
                ShowAddressBook = true
            });

            return newCourseOffering;
        }

        private static async Task DeleteCourseOfferingAsync(IApiClient apiClient, CourseOffering courseOffering)
        {
            await apiClient.DeleteCourseOfferingAsync((int)courseOffering.Identifier!, permanently: true);
            await apiClient.DeleteCourseTemplateAsync((int)courseOffering.CourseTemplate!.Identifier!, permanently: true);
        }
    }
}
