using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;

namespace Resourcerer.DataAccess.Configurations;

internal class InstanceDiscardedEventConfiguration : IEntityTypeConfiguration<InstanceDiscardedEvent>
{
    public void Configure(EntityTypeBuilder<InstanceDiscardedEvent> builder)
    {
        AppDbContext.ConfigureEntity(builder, e =>
        {
            e.HasOne(x => x.Instance).WithMany(x => x.DiscardedEvents)
                .HasForeignKey(x => x.InstanceId)
                .HasConstraintName($"FK_{nameof(Instance)}_{nameof(InstanceDiscardedEvent)}");
        });
    }
}
