using HR.BrightspaceConnector.Features.Courses;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.BrightspaceConnector.Infrastructure.Persistence.Configurations
{
    internal class CourseOfferingRecordConfiguration : IEntityTypeConfiguration<CourseOfferingRecord>
    {
        public void Configure(EntityTypeBuilder<CourseOfferingRecord> builder)
        {
            builder.HasNoKey();
            builder.Property(co => co.SyncInternalKey).HasConversion<int>();
            builder.Property(co => co.SemesterId).HasConversion<short>();
            builder.Property(co => co.IsActive).HasConversion<int>();
            builder.Property(co => co.ForceLocale).HasConversion<int>();
            builder.Property(co => co.ShowAddressBook).HasConversion<int>();
            builder.Property(co => co.CanSelfRegister).HasConversion<int>();

            // TODO: Not sure how these are supposed to be mapped.
            // Defined as int columns in the database while the corresponding object properties are of type DateTimeOffset.
            builder.Ignore(co => co.StartDate);
            builder.Ignore(co => co.EndDate);

            builder.Property(co => co.CourseTemplateId).HasConversion(
                convertToProviderExpression: (int? value) => value != null ? ((int)value).ToString() : null,
                convertFromProviderExpression: (string? value) => !string.IsNullOrEmpty(value) ? value.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Select(s => int.Parse(s)).First() : null);
        }
    }
}
