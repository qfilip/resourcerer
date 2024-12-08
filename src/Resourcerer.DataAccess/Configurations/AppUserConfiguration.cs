using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;

namespace Resourcerer.DataAccess.Configurations;

internal sealed class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        AppDbContext.ConfigureEntity(builder, (e) =>
        {
            e.Property(x => x.Name).IsRequired();
            e.HasIndex(x => x.Email).IsUnique();

            e.HasOne(x => x.Company).WithMany(x => x.Employees)
                .HasForeignKey(x => x.CompanyId)
                .HasConstraintName($"FK_{nameof(Company)}_{nameof(AppUser)}");
        });
    }
}
