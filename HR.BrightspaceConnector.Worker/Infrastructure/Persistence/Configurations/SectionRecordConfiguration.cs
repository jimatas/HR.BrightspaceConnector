using HR.BrightspaceConnector.Features.Sections;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.BrightspaceConnector.Infrastructure.Persistence.Configurations
{
    internal class SectionRecordConfiguration : IEntityTypeConfiguration<SectionRecord>
    {
        public void Configure(EntityTypeBuilder<SectionRecord> builder)
        {
            builder.HasNoKey();
            builder.Property(s => s.SyncInternalKey).HasConversion<int>();
            builder.Property(e => e.OrgUnitId).HasConversion<string>();
        }
    }
}
