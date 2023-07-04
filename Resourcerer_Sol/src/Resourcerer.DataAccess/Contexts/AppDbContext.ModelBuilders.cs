using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Enums;

namespace Resourcerer.DataAccess.Contexts;
public partial class AppDbContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(e =>
        {
            e.ToTable(nameof(Category));
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.Name).IsUnique();
            e.Property(x => x.Name).IsRequired();
            e.HasOne(x => x.ParentCategory)
                .WithMany(x => x.ChildCategories);
            e.HasQueryFilter(x => x.EntityStatus != eEntityStatus.Deleted);
        });

        modelBuilder.Entity<Composite>(e =>
        {
            e.ToTable(nameof(Composite));
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.Name).IsUnique();
            e.Property(x => x.Name).IsRequired();
            e.HasOne(x => x.Category).WithMany(x => x.Composites);
            e.HasQueryFilter(x => x.EntityStatus != eEntityStatus.Deleted);
        });

        modelBuilder.Entity<CompositeSoldEvent>(e =>
        {
            e.ToTable(nameof(CompositeSoldEvent));
            e.HasKey(x => x.Id);
        });

        modelBuilder.Entity<Element>(e =>
        {
            e.ToTable(nameof(Element));
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.Name).IsUnique();
            e.Property(x => x.Name).IsRequired();
            e.HasOne(x => x.Category).WithMany(x => x.Elements);
            e.HasOne(x => x.UnitOfMeasure).WithMany(x => x.Elements);
            e.HasQueryFilter(x => x.EntityStatus != eEntityStatus.Deleted);
        });

        modelBuilder.Entity<ElementInstance>(e =>
        {
            e.ToTable(nameof(ElementInstance));
            e.HasKey(x => x.Id);
            e.HasOne(x => x.Element).WithMany(x => x.ElementInstances);
            e.HasQueryFilter(x => x.EntityStatus != eEntityStatus.Deleted);
        });

        modelBuilder.Entity<ElementPurchasedEvent>(e =>
        {
            e.ToTable(nameof(ElementPurchasedEvent));
            e.HasKey(x => x.Id);
        });

        modelBuilder.Entity<ElementDeliveredEvent>(e =>
        {
            e.ToTable(nameof(ElementPurchasedEvent));
            e.HasKey(x => x.Id);
        });

        modelBuilder.Entity<ElementSoldEvent>(e =>
        {
            e.ToTable(nameof(ElementSoldEvent));
            e.HasKey(x => x.Id);
        });

        modelBuilder.Entity<Price>(e =>
        {
            e.ToTable(nameof(Price));
            e.HasKey(x => x.Id);
            e.Property(x => x.UnitValue).IsRequired();
            e.HasOne(x => x.Element).WithMany(x => x.Prices);
            e.HasOne(x => x.Composite).WithMany(x => x.Prices);
            e.HasQueryFilter(x => x.EntityStatus != eEntityStatus.Deleted);
        });

        modelBuilder.Entity<Excerpt>(e =>
        {
            e.ToTable(nameof(Excerpt));
            e.HasKey(x => x.Id);
            e.HasOne(x => x.Composite).WithMany(x => x.Excerpts);
            e.HasOne(x => x.Element).WithMany(x => x.Excerpts);
            e.HasQueryFilter(x => x.EntityStatus != eEntityStatus.Deleted);
        });

        modelBuilder.Entity<UnitOfMeasure>(e =>
        {
            e.ToTable(nameof(UnitOfMeasure));
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.Name).IsUnique();
            e.Property(x => x.Name).IsRequired();
            e.Property(x => x.Symbol).IsRequired();
            e.HasQueryFilter(x => x.EntityStatus != eEntityStatus.Deleted);
        });

        modelBuilder.Entity<AppUser>(e =>
        {
            e.ToTable(nameof(AppUser));
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.Name).IsUnique();
            e.Property(x => x.Name).IsRequired();
            e.HasQueryFilter(x => x.EntityStatus != eEntityStatus.Deleted);
        });

        base.OnModelCreating(modelBuilder);
    }
}

