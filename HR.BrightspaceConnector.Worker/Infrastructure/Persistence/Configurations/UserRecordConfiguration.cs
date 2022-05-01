using HR.BrightspaceConnector.Features.Users;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.BrightspaceConnector.Infrastructure.Persistence.Configurations
{
    internal class UserRecordConfiguration : IEntityTypeConfiguration<UserRecord>
    {
        public void Configure(EntityTypeBuilder<UserRecord> builder)
        {
            builder.HasNoKey();
            builder.Property(u => u.IsActive).HasConversion<int>();
            builder.Property(u => u.SendCreationEmail).HasConversion<int>();
        }
    }
}
