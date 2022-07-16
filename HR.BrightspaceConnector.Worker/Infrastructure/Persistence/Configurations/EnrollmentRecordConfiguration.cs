using HR.BrightspaceConnector.Features.Enrollments;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.BrightspaceConnector.Infrastructure.Persistence.Configurations
{
    internal class EnrollmentRecordConfiguration : IEntityTypeConfiguration<EnrollmentRecord>
    {
        public void Configure(EntityTypeBuilder<EnrollmentRecord> builder)
        {
            builder.HasNoKey();
            builder.Property(e => e.SyncInternalKey).HasConversion<int>();
            builder.Property(e => e.OrgUnitId).HasConversion<string>();
            builder.Property(e => e.UserId).HasConversion<string>();
        }
    }
}
