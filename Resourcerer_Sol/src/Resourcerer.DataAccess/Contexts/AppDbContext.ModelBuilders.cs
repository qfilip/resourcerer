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
            e.HasIndex(x => x.Name).IsUnique();
            e.Property(x => x.Name).IsRequired();
            e.HasOne(x => x.ParentCategory)
                .WithMany(x => x.ChildCategories);
        });

        modelBuilder.Entity<Composite>(e =>
        {
            e.ToTable(nameof(Composite));
            e.HasIndex(x => x.Name).IsUnique();
            e.Property(x => x.Name).IsRequired();
            e.HasOne(x => x.Category).WithMany(x => x.Composites);
        });

        modelBuilder.Entity<CompositeSoldEvent>(e =>
        {
            e.ToTable(nameof(CompositeSoldEvent));
            e.HasOne(x => x.Composite).WithMany(x => x.CompositeSoldEvents);
        });

        modelBuilder.Entity<Element>(e =>
        {
            e.ToTable(nameof(Element));
            e.HasIndex(x => x.Name).IsUnique();
            e.Property(x => x.Name).IsRequired();
            e.HasOne(x => x.Category).WithMany(x => x.Elements);
            e.HasOne(x => x.UnitOfMeasure).WithMany(x => x.Elements);
        });

        modelBuilder.Entity<ElementPurchasedEvent>(e =>
        {
            e.ToTable(nameof(ElementPurchasedEvent));
            e.HasOne(x => x.Element).WithMany(x => x.ElementPurchasedEvents);
        });

        modelBuilder.Entity<ElementSoldEvent>(e =>
        {
            e.ToTable(nameof(ElementSoldEvent));
            e.HasOne(x => x.Element).WithMany(x => x.ElementSoldEvents);
         });

        modelBuilder.Entity<Price>(e =>
        {
            e.ToTable(nameof(Price));
            e.Property(x => x.Value).IsRequired();
            e.HasOne(x => x.Element).WithMany(x => x.Prices);
            e.HasOne(x => x.Composite).WithMany(x => x.Prices);
        });

        modelBuilder.Entity<Excerpt>(e =>
        {
            e.ToTable(nameof(Excerpt));
            e.HasOne(x => x.Composite).WithMany(x => x.Excerpts);
            e.HasOne(x => x.Element).WithMany(x => x.Excerpts);
        });

        modelBuilder.Entity<UnitOfMeasure>(e =>
        {
            e.ToTable(nameof(UnitOfMeasure));
            e.HasIndex(x => x.Name).IsUnique();
            e.Property(x => x.Name).IsRequired();
            e.Property(x => x.Symbol).IsRequired();
        });

        modelBuilder.Entity<AppUser>(e =>
        {
            e.ToTable(nameof(AppUser));
            e.HasIndex(x => x.Name).IsUnique();
            e.Property(x => x.Name).IsRequired();
        });

        base.OnModelCreating(modelBuilder);
    }
}

