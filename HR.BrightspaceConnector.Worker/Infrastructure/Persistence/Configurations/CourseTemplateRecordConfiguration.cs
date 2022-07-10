using HR.BrightspaceConnector.Features.Courses;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.BrightspaceConnector.Infrastructure.Persistence.Configurations
{
    internal class CourseTemplateRecordConfiguration : IEntityTypeConfiguration<CourseTemplateRecord>
    {
        public void Configure(EntityTypeBuilder<CourseTemplateRecord> builder)
        {
            builder.HasNoKey();
            builder.Property(ct => ct.SyncInternalKey).HasConversion<int>();
            builder.Property(ct => ct.ParentOrgUnitIds).IsRequired(false).HasConversion(
                convertToProviderExpression: (IEnumerable<int> values) => values.Any() ? string.Join(',', values) : null,
                convertFromProviderExpression: (string? value) => !string.IsNullOrEmpty(value) ? value.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Select(s => int.Parse(s)) : Enumerable.Empty<int>(),
                valueComparer: new ValueComparer<IEnumerable<int>>(
                    equalsExpression: (x, y) => (x != null && y != null && x.SequenceEqual(y)) || ReferenceEquals(x, y),
                    hashCodeExpression: values => values.Aggregate(0, (hashCode, value) => HashCode.Combine(hashCode, value)),
                    snapshotExpression: values => values.ToHashSet()));
        }
    }
}
