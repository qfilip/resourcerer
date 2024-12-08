using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;

namespace Resourcerer.DataAccess.Configurations;

internal class RecipeConfiguration : IEntityTypeConfiguration<Recipe>
{
    public void Configure(EntityTypeBuilder<Recipe> builder)
    {
        AppDbContext.ConfigureEntity(builder, e =>
        {
            e.HasOne(x => x.CompositeItem).WithMany(x => x.Recipes)
                .HasForeignKey(x => x.CompositeItemId)
                .IsRequired(true)
                .HasConstraintName($"FK_{nameof(Item)}_{nameof(Recipe)}");

            e.HasIndex(x => new { x.CompositeItemId, x.Version })
                .IsUnique();
        });
    }
}
