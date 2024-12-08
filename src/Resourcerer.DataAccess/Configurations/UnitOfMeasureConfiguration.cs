using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;

namespace Resourcerer.DataAccess.Configurations;

internal class UnitOfMeasureConfiguration : IEntityTypeConfiguration<UnitOfMeasure>
{
    public void Configure(EntityTypeBuilder<UnitOfMeasure> builder)
    {
        AppDbContext.ConfigureEntity(builder, e =>
        {
            e.HasIndex(x => x.Name).IsUnique();
            e.Property(x => x.Name).IsRequired();
            e.Property(x => x.Symbol).IsRequired();

            e.HasOne(x => x.Company).WithMany(x => x.UnitsOfMeasure)
                .HasForeignKey(x => x.CompanyId)
                .HasConstraintName($"FK_Element{nameof(Company)}_{nameof(UnitOfMeasure)}");
        });
    }
}
