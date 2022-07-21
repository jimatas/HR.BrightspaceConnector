using HR.BrightspaceConnector.Infrastructure.Persistence;
using HR.Common.Cqrs.Queries;
using HR.Common.Utilities;

namespace HR.BrightspaceConnector.Features.Sections.Queries
{
    public class GetNextSection : IQuery<SectionRecord?>
    {
    }

    public class GetNextSectionHandler : IQueryHandler<GetNextSection, SectionRecord?>
    {
        private readonly IDatabase database;
        private readonly ILogger logger;

        public GetNextSectionHandler(IDatabase database, ILogger<GetNextSection> logger)
        {
            this.database = database;
            this.logger = logger;
        }

        public async Task<SectionRecord?> HandleAsync(GetNextSection query, CancellationToken cancellationToken)
        {
            SectionRecord? section = await database.GetNextCourseOfferingSectionAsync(cancellationToken).WithoutCapturingContext();
            if (section is not null)
            {
                logger.LogInformation("Retrieved section with code \"{SectionCode}\" for sync action '{SyncAction}' from database.", section.Code, section.SyncAction);
            }

            return section;
        }
    }
}
