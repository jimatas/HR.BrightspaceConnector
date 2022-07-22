using HR.BrightspaceConnector.Features.Common.Commands;
using HR.BrightspaceConnector.Utilities;
using HR.Common.Cqrs.Commands;
using HR.Common.Utilities;

namespace HR.BrightspaceConnector.Features.Sections.Commands
{
    public class DeleteSection : ICommand
    {
        public DeleteSection(SectionRecord section)
        {
            Section = section;
        }

        public SectionRecord Section { get; }
    }

    public class DeleteSectionHandler : ICommandHandler<DeleteSection>
    {
        private readonly IApiClient apiClient;
        private readonly ICommandDispatcher commandDispatcher;
        private readonly ILogger logger;

        public DeleteSectionHandler(IApiClient apiClient, ICommandDispatcher commandDispatcher, ILogger<DeleteSection> logger)
        {
            this.apiClient = apiClient;
            this.commandDispatcher = commandDispatcher;
            this.logger = logger;
        }

        public async Task HandleAsync(DeleteSection command, CancellationToken cancellationToken)
        {
            SectionRecord section = command.Section;
            int orgUnitId = (int)section.OrgUnitId!;
            int sectionId = Convert.ToInt32(section.SyncExternalKey);
            int eventId = (int)section.SyncEventId!;

            logger.LogInformation("Deleting section with code \"{SectionCode}\" from Brightspace.", section.Code);

            try
            {
                await apiClient.DeleteSectionAsync(orgUnitId, sectionId, cancellationToken).WithoutCapturingContext();
                logger.LogInformation("Section was successfully deleted.");

                await commandDispatcher.DispatchAsync(MarkAsHandled.Successfully(eventId, sectionId), cancellationToken).WithoutCapturingContext();
            }
            catch (ApiException exception)
            {
                logger.LogWarning(exception, "Error while deleting section. {ErrorMessage}", exception.GetErrorMessage());

                await commandDispatcher.DispatchAsync(MarkAsHandled.Unsuccessfully(eventId, sectionId, exception.GetErrorMessage()), cancellationToken).WithoutCapturingContext();
            }
        }
    }
}
