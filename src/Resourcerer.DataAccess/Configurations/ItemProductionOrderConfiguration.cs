using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;

namespace Resourcerer.DataAccess.Configurations
{
    internal class ItemProductionOrderConfiguration : IEntityTypeConfiguration<ItemProductionOrder>
    {
        public void Configure(EntityTypeBuilder<ItemProductionOrder> builder)
        {
            AppDbContext.ConfigureEntity(builder, e =>
            {
                e.HasOne(x => x.Item).WithMany(x => x.ProductionOrders)
                    .HasForeignKey(x => x.ItemId).IsRequired()
                    .HasConstraintName($"FK_{nameof(Item)}_{nameof(ItemProductionOrder)}");

                e.OwnsOne(x => x.StartedEvent, nav =>
                {
                    nav.OwnsOne(n => n.AuditRecord, nb => nb.ToJson());
                    nav.ToJson();
                });

                e.OwnsOne(x => x.CancelledEvent, nav =>
                {
                    nav.OwnsOne(n => n.AuditRecord, nb => nb.ToJson());
                    nav.ToJson();
                });

                e.OwnsOne(x => x.FinishedEvent, nav =>
                {
                    nav.OwnsOne(n => n.AuditRecord, nb => nb.ToJson());
                    nav.ToJson();
                });
            });
        }
    }
}
