using HR.BrightspaceConnector.Features.Common.Commands;
using HR.BrightspaceConnector.Utilities;
using HR.Common.Cqrs.Commands;
using HR.Common.Utilities;

namespace HR.BrightspaceConnector.Features.Sections.Commands
{
    public class UpdateSection : ICommand
    {
        public UpdateSection(SectionRecord section)
        {
            Section = section;
        }

        public SectionRecord Section { get; }
    }

    public class UpdateSectionHandler : ICommandHandler<UpdateSection>
    {
        private readonly IApiClient apiClient;
        private readonly ICommandDispatcher commandDispatcher;
        private readonly ILogger logger;

        public UpdateSectionHandler(IApiClient apiClient, ICommandDispatcher commandDispatcher, ILogger<UpdateSection> logger)
        {
            this.apiClient = apiClient;
            this.commandDispatcher = commandDispatcher;
            this.logger = logger;
        }

        public async Task HandleAsync(UpdateSection command, CancellationToken cancellationToken)
        {
            SectionRecord section = command.Section;
            int orgUnitId = (int)section.OrgUnitId!;
            int sectionId = Convert.ToInt32(section.SyncExternalKey);
            int eventId = (int)section.SyncEventId!;

            logger.LogInformation("Updating section with code \"{SectionCode}\" in Brightspace.", section.Code);

            try
            {
                var updatedSection = await apiClient.UpdateSectionAsync(orgUnitId, sectionId, section.ToCreateOrUpdateSectionData(), cancellationToken).WithoutCapturingContext();
                logger.LogInformation("Section was successfully updated.");

                await commandDispatcher.DispatchAsync(MarkAsHandled.Successfully(eventId, sectionId), cancellationToken).WithoutCapturingContext();
            }
            catch (ApiException exception)
            {
                logger.LogWarning(exception, "Error while updating section. {ErrorMessage}", exception.GetErrorMessage());

                await commandDispatcher.DispatchAsync(MarkAsHandled.Unsuccessfully(eventId, sectionId, exception.GetErrorMessage()), cancellationToken).WithoutCapturingContext();
            }
        }
    }
}
