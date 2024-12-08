using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;

namespace Resourcerer.DataAccess.Configurations;

internal class InstanceOrderedEventConfiguration : IEntityTypeConfiguration<InstanceOrderedEvent>
{
    public void Configure(EntityTypeBuilder<InstanceOrderedEvent> builder)
    {
        AppDbContext.ConfigureEntity(builder, e =>
        {
            e.HasOne(x => x.Instance).WithMany(x => x.OrderedEvents)
                .HasForeignKey(x => x.InstanceId)
                .HasConstraintName($"FK_{nameof(Instance)}_{nameof(InstanceOrderedEvent)}");

            e.OwnsOne(x => x.CancelledEvent, nav =>
            {
                nav.OwnsOne(n => n.AuditRecord, nb => nb.ToJson());
                nav.ToJson();
            });
            e.OwnsOne(x => x.SentEvent, nav =>
            {
                nav.OwnsOne(n => n.AuditRecord, nb => nb.ToJson());
                nav.ToJson();
            });
            e.OwnsOne(x => x.DeliveredEvent, nav =>
            {
                nav.OwnsOne(n => n.AuditRecord, nb => nb.ToJson());
                nav.ToJson();
            });
        });
    }
}
