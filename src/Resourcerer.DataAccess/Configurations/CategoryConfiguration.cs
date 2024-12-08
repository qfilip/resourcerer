using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;

namespace Resourcerer.DataAccess.Configurations;

internal class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        AppDbContext.ConfigureEntity(builder, e =>
        {
            e.Property(x => x.Name).IsRequired();

            e.HasOne(x => x.Company).WithMany(x => x.Categories)
                .HasForeignKey(x => x.CompanyId)
                .HasConstraintName($"FK_{nameof(Category)}_{nameof(Company)}");

            e.HasOne(x => x.ParentCategory).WithMany(x => x.ChildCategories)
                .HasForeignKey(x => x.ParentCategoryId)
                .IsRequired(false)
                .HasConstraintName($"FK_{nameof(Category)}_{nameof(Category)}");
        });
    }
}
