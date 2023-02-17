using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Entities;

namespace Resourcerer.DataAccess.Contexts;
public partial class AppDbContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(e =>
        {
            e.ToTable(nameof(Category));
            e.Property(x => x.Name).IsRequired();
            e.HasOne(x => x.ParentCategory)
                .WithMany(x => x.Categories);
        });

        modelBuilder.Entity<Composite>(e =>
        {
            e.ToTable(nameof(Composite));
            e.Property(x => x.Name).IsRequired();
            e.HasOne(x => x.Category).WithMany(x => x.Composites);
        });

        modelBuilder.Entity<Element>(e =>
        {
            e.ToTable(nameof(Element));
            e.Property(x => x.Name).IsRequired();
            e.HasOne(x => x.Category).WithMany(x => x.Elements);
        });

        modelBuilder.Entity<Excerpt>(e =>
        {
            e.ToTable(nameof(Excerpt));
            e.HasOne(x => x.Composite).WithMany(x => x.Excerpts);
            e.HasOne(x => x.Element).WithMany(x => x.Excerpts);
            e.HasOne(x => x.UnitOfMeasure).WithMany(x => x.Excerpts);
        });

        modelBuilder.Entity<Price>(e =>
        {
            e.ToTable(nameof(Price));
            e.Property(x => x.ValidFrom).IsRequired();
            e.Property(x => x.Value).IsRequired();
            e.HasOne(x => x.Composite).WithOne(x => x.CurrentPrice);
            e.HasOne(x => x.Element).WithOne(x => x.CurrentPrice);
        });

        modelBuilder.Entity<UnitOfMeasure>(e =>
        {
            e.ToTable(nameof(UnitOfMeasure));
            e.Property(x => x.Name).IsRequired();
            e.Property(x => x.Abbreviation).IsRequired();
        });

        modelBuilder.Entity<AppUser>(e =>
        {
            e.ToTable(nameof(AppUser));
            e.Property(x => x.Name).IsRequired();
        });

        modelBuilder.Entity<AppEvent>(e =>
        {
            e.ToTable(nameof(AppEvent));
            e.Property(x => x.EventType).IsRequired();
            e.Property(x => x.Data).IsRequired();
        });

        base.OnModelCreating(modelBuilder);
    }
}

