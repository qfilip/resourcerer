using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;

namespace Resourcerer.DataAccess.Configurations;

internal class PriceConfiguration : IEntityTypeConfiguration<Price>
{
    public void Configure(EntityTypeBuilder<Price> builder)
    {
        AppDbContext.ConfigureEntity(builder, e =>
        {
            e.Property(x => x.UnitValue).IsRequired();

            e.HasOne(x => x.Item).WithMany(x => x.Prices)
                .HasForeignKey(x => x.ItemId)
                .HasConstraintName($"FK_{nameof(Item)}_{nameof(Price)}");
        });
    }
}
