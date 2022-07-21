using HR.BrightspaceConnector.Features.Sections.Commands;
using HR.Common.Cqrs;
using HR.Common.Cqrs.Commands;
using HR.Common.Utilities;

using System.Net;

namespace HR.BrightspaceConnector.Features.Sections.Decorators
{
    /// <summary>
    /// Handles the on-demand initialization of sections for a course offering.
    /// </summary>
    public class CreateSectionSettings : ICommandHandlerWrapper<CreateSection>
    {
        private IApiClient apiClient;

        public CreateSectionSettings(IApiClient apiClient)
        {
            this.apiClient = apiClient;
        }

        public async Task HandleAsync(CreateSection command, HandlerDelegate next, CancellationToken cancellationToken)
        {
            try
            {
                // Request section settings; if sections have not yet been initialized for the current course offering, a 404 will be returned instead.
                // When that happens, catch the resulting exception and create the section settings.
                await apiClient.GetSectionSettingsAsync((int)command.Section.OrgUnitId!, cancellationToken).WithoutCapturingContext();
            }
            catch (ApiException exception) when (exception.StatusCode == HttpStatusCode.NotFound)
            {
                await apiClient.CreateSectionSettingsAsync((int)command.Section.OrgUnitId!,
                    new CreateSectionSettingsData
                    {
                        AutoEnroll = false,
                        DescriptionsVisibleToEnrollees = true,
                        EnrollmentQuantity = 20,
                        EnrollmentStyle = SectionEnrollmentStyle.PeoplePerSectionAutoEnrollment,
                        RandomizeEnrollments = false
                    },
                    cancellationToken).WithoutCapturingContext();
            }

            await next().WithoutCapturingContext();
        }
    }
}
