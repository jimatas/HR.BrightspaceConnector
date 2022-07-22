using HR.BrightspaceConnector.Features.Common.Commands;
using HR.BrightspaceConnector.Utilities;
using HR.Common.Cqrs.Commands;
using HR.Common.Utilities;

namespace HR.BrightspaceConnector.Features.Sections.Commands
{
    public class CreateSection : ICommand
    {
        public CreateSection(SectionRecord section)
        {
            Section = section;
        }

        public SectionRecord Section { get; }
    }

    public class CreateSectionHandler : ICommandHandler<CreateSection>
    {
        private readonly IApiClient apiClient;
        private readonly ICommandDispatcher commandDispatcher;
        private readonly ILogger logger;

        public CreateSectionHandler(IApiClient apiClient, ICommandDispatcher commandDispatcher, ILogger<CreateSection> logger)
        {
            this.apiClient = apiClient;
            this.commandDispatcher = commandDispatcher;
            this.logger = logger;
        }

        public async Task HandleAsync(CreateSection command, CancellationToken cancellationToken)
        {
            SectionRecord section = command.Section;
            int orgUnitId = (int)section.OrgUnitId!;
            int eventId = (int)section.SyncEventId!;

            logger.LogInformation("Creating section with code \"{SectionCode}\" in Brightspace.", section.Code);

            try
            {
                var newSection = await apiClient.CreateSectionAsync(orgUnitId, section.ToCreateOrUpdateSectionData(), cancellationToken).WithoutCapturingContext();
                logger.LogInformation("Section was successfully created.");

                await commandDispatcher.DispatchAsync(MarkAsHandled.Successfully(eventId, (int)newSection.SectionId!), cancellationToken).WithoutCapturingContext();
            }
            catch (ApiException exception)
            {
                logger.LogWarning(exception, "Error while creating section. {ErrorMessage}", exception.GetErrorMessage());

                await commandDispatcher.DispatchAsync(MarkAsHandled.Unsuccessfully(eventId, exception.GetErrorMessage()), cancellationToken).WithoutCapturingContext();
            }
        }
    }
}
