using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;

namespace Resourcerer.DataAccess.Configurations;

internal class InstanceConfiguration : IEntityTypeConfiguration<Instance>
{
    public void Configure(EntityTypeBuilder<Instance> builder)
    {
        AppDbContext.ConfigureEntity(builder, e =>
        {
            e.HasOne(x => x.Item).WithMany(x => x.Instances)
                .HasForeignKey(x => x.ItemId)
                .HasConstraintName($"FK_{nameof(Item)}_{nameof(Instance)}");

            e.HasOne(x => x.OwnerCompany).WithMany(x => x.Instances)
                .HasForeignKey(x => x.OwnerCompanyId)
                .HasConstraintName($"FK_{nameof(Company)}_{nameof(Instance)}");

            e.HasOne(x => x.SourceInstance).WithMany(x => x.DerivedInstances)
                .HasForeignKey(x => x.SourceInstanceId)
                .HasConstraintName($"FK_{nameof(Instance)}_{nameof(Instance)}");
        });
    }
}
