using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;

namespace Resourcerer.DataAccess.Configurations;

internal class ItemConfiguration : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> builder)
    {
        AppDbContext.ConfigureEntity(builder, e =>
        {
            e.Property(x => x.Name).IsRequired();

            e.HasOne(x => x.Category).WithMany(x => x.Items)
                .HasForeignKey(x => x.CategoryId).IsRequired()
                .HasConstraintName($"FK_{nameof(Category)}_{nameof(Item)}");

            e.HasOne(x => x.UnitOfMeasure).WithMany(x => x.Items)
                .HasForeignKey(x => x.UnitOfMeasureId).IsRequired()
                .HasConstraintName($"FK_{nameof(UnitOfMeasure)}_{nameof(Item)}");
        });
    }
}
