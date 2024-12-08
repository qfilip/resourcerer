using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;

namespace Resourcerer.DataAccess.Configurations
{
    internal class InstanceReservedEventConfiguration : IEntityTypeConfiguration<InstanceReservedEvent>
    {
        public void Configure(EntityTypeBuilder<InstanceReservedEvent> builder)
        {
            AppDbContext.ConfigureEntity(builder, e =>
            {
                e.HasOne(x => x.Instance).WithMany(x => x.ReservedEvents)
                    .HasForeignKey(x => x.InstanceId)
                    .HasConstraintName($"FK_{nameof(Instance)}_{nameof(InstanceReservedEvent)}");

                e.OwnsOne(x => x.CancelledEvent, nav =>
                {
                    nav.OwnsOne(n => n.AuditRecord, nb => nb.ToJson());
                    nav.ToJson();
                });

                e.OwnsOne(x => x.UsedEvent, nav =>
                {
                    nav.OwnsOne(n => n.AuditRecord, nb => nb.ToJson());
                    nav.ToJson();
                });
            });
        }
    }
}
