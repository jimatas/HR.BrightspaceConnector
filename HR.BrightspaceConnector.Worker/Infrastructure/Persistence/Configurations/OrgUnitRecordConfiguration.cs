using HR.BrightspaceConnector.Features.OrgUnits;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.BrightspaceConnector.Infrastructure.Persistence.Configurations
{
    internal class OrgUnitRecordConfiguration : IEntityTypeConfiguration<OrgUnitRecord>
    {
        public void Configure(EntityTypeBuilder<OrgUnitRecord> builder)
        {
            builder.HasNoKey();
            builder.Property(ou => ou.Parent).HasColumnName("Parents");
        }
    }
}
