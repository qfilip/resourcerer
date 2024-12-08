using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;

namespace Resourcerer.DataAccess.Configurations;

internal class RecipeExcerptConfiguration : IEntityTypeConfiguration<RecipeExcerpt>
{
    public void Configure(EntityTypeBuilder<RecipeExcerpt> builder)
    {
        AppDbContext.ConfigureEntity(builder, e =>
        {
            e.HasKey(x => new { x.RecipeId, x.ElementId });

            e.HasOne(x => x.Recipe).WithMany(x => x.RecipeExcerpts)
                .HasForeignKey(x => x.RecipeId)
                .IsRequired()
                .HasConstraintName($"FK_Composite{nameof(Recipe)}_{nameof(RecipeExcerpt)}");

            e.HasOne(x => x.Element).WithMany(x => x.ElementRecipeExcerpts)
                .HasForeignKey(x => x.ElementId)
                .IsRequired()
                .HasConstraintName($"FK_Element{nameof(Item)}_{nameof(RecipeExcerpt)}");
        });
    }
}
