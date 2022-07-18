using HR.BrightspaceConnector.Features.Common;
using HR.BrightspaceConnector.Features.Courses;
using HR.BrightspaceConnector.Features.Sections;
using HR.BrightspaceConnector.Utilities;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Net;
using System.Threading.Tasks;

namespace HR.BrightspaceConnector.Tests
{
    [TestClass]
    public class SectionsTests : IntegrationTestsBase
    {
        [TestMethod]
        public async Task CreateSections()
        {
            var apiClient = CreateApiClient();

            CourseOffering courseOffering = await CreateCourseOfferingAsync(apiClient);
            Assert.IsNotNull(courseOffering.Identifier);

            try
            {
                try
                {
                    var sectionSettings = await apiClient.GetSectionSettingsAsync((int)courseOffering.Identifier);
                }
                catch (ApiException exception) when (exception.StatusCode == HttpStatusCode.NotFound)
                {
                    var sectionSettingsToCreate = new CreateSectionSettingsData
                    {
                        AutoEnroll = true,
                        DescriptionsVisibleToEnrollees = true,
                        EnrollmentQuantity = 3,
                        EnrollmentStyle = SectionEnrollmentStyle.NumberOfSectionsAutoEnrollment,
                        RandomizeEnrollments = true
                    };

                    var newSectionSettings = await apiClient.CreateSectionSettingsAsync((int)courseOffering.Identifier, sectionSettingsToCreate);

                    var sectionSettingsToUpdate = new UpdateSectionSettingsData
                    {
                        AutoEnroll = false,
                        DescriptionsVisibleToEnrollees = false,
                        RandomizeEnrollments = false,
                        Name = "MySuperSection",
                        Description = new RichTextInput
                        {
                            Content = "These are my super sections",
                            Type = TextContentType.Text
                        }
                    };

                    var newerSectionSettings = await apiClient.UpdateSectionSettingsAsync((int)courseOffering.Identifier, sectionSettingsToUpdate);
                }

                var newSection = await apiClient.CreateSectionAsync((int)courseOffering.Identifier,
                    new CreateOrUpdateSectionData
                    {
                        Code = "Section-4",
                        Name = "Sample Section 4",
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

                var sectionsToDelete = await apiClient.GetSectionsAsync((int)courseOffering.Identifier);
                foreach (var sectionToDelete in sectionsToDelete)
                {
                    await apiClient.DeleteSectionAsync((int)courseOffering.Identifier, (int)sectionToDelete.SectionId!);
                }
            }
            catch (ApiException exception)
            {

            }
            finally
            {
                await DeleteCourseOfferingAsync(apiClient, courseOffering);
            }
        }
        
        private async Task<CourseOffering> CreateCourseOfferingAsync(IApiClient apiClient)
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

        private async Task DeleteCourseOfferingAsync(IApiClient apiClient, CourseOffering courseOffering)
        {
            await apiClient.DeleteCourseOfferingAsync((int)courseOffering.Identifier!, permanently: true);
            await apiClient.DeleteCourseTemplateAsync((int)courseOffering.CourseTemplate!.Identifier!, permanently: true);
        }
    }
}
