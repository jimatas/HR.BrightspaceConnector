using HR.BrightspaceConnector.Features.OrgUnits;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.BrightspaceConnector.Infrastructure.Persistence.Configurations
{
    internal class OrgUnitRecordConfiguration : IEntityTypeConfiguration<OrgUnitRecord>
    {
        public void Configure(EntityTypeBuilder<OrgUnitRecord> builder)
        {
            builder.HasNoKey();
            builder.Property(ou => ou.Parents).HasConversion(
                convertToProviderExpression: (IEnumerable<int> parents) => parents.Any() ? parents.First() : null,
                convertFromProviderExpression: (int? parent) => parent.HasValue ? new[] { parent.Value } : Enumerable.Empty<int>(),
                valueComparer: new ValueComparer<IEnumerable<int>>(
                    equalsExpression: (x, y) => (x != null && y != null && x.SequenceEqual(y)) || x == y,
                    hashCodeExpression: values => values.Aggregate(0, (hashCode, value) => HashCode.Combine(hashCode, value)),
                    snapshotExpression: values => values.ToHashSet()));
        }
    }
}
