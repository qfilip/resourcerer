using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Enums;

namespace Resourcerer.DataAccess.Contexts;
public partial class AppDbContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureEntity<Category>(modelBuilder, e =>
        {
            e.HasIndex(x => x.Name).IsUnique();
            e.Property(x => x.Name).IsRequired();
            e.HasOne(x => x.ParentCategory).WithMany(x => x.ChildCategories);
        });

        ConfigureEntity<Composite>(modelBuilder, e =>
        {
            e.HasIndex(x => x.Name).IsUnique();
            e.Property(x => x.Name).IsRequired();
            e.HasOne(x => x.Category).WithMany(x => x.Composites);
            e.HasOne(x => x.UnitOfMeasure).WithMany(x => x.Composites);
        });

        ConfigureEntity<CompositeSoldEvent>(modelBuilder, e =>
        {
            e.HasOne(x => x.Composite).WithMany(x => x.CompositeSoldEvents);
        });

        ConfigureEntity<Element>(modelBuilder, e =>
        {
            e.HasIndex(x => x.Name).IsUnique();
            e.Property(x => x.Name).IsRequired();
            e.HasOne(x => x.Category).WithMany(x => x.Elements);
            e.HasOne(x => x.UnitOfMeasure).WithMany(x => x.Elements);
        });

        ConfigureEntity<ElementPurchasedEvent>(modelBuilder, e =>
        {
            e.HasOne(x => x.Element).WithMany(x => x.ElementPurchasedEvents);
        });

        ConfigureEntity<ElementPurchaseCancelledEvent>(modelBuilder, e =>
        {
            e.HasOne(x => x.ElementPurchasedEvent).WithOne(x => x.ElementPurchaseCancelledEvent);
        });

        ConfigureEntity<ElementDeliveredEvent>(modelBuilder, e =>
        {
            e.HasOne(x => x.ElementPurchasedEvent).WithOne(x => x.ElementDeliveredEvent);
        });

        ConfigureEntity<ElementInstanceDiscardedEvent>(modelBuilder);

        ConfigureEntity<ElementInstanceSoldEvent>(modelBuilder, e =>
        {
            e.HasOne(x => x.ElementInstance).WithMany(x => x.ElementInstanceSoldEvents);
        });

        ConfigureEntity<Price>(modelBuilder, (e) =>
        {
            e.Property(x => x.UnitValue).IsRequired();
            e.HasOne(x => x.Element).WithMany(x => x.Prices);
            e.HasOne(x => x.Composite).WithMany(x => x.Prices);
        });

        ConfigureEntity<Instance>(modelBuilder, (e) =>
        {
            e.Property(x => x.Quantity).IsRequired();
            e.HasOne(x => x.Element).WithMany(x => x.ElementInstances);
            e.HasOne(x => x.Composite).WithMany(x => x.CompositeInstances);
        });

        ConfigureEntity<Excerpt>(modelBuilder, (e) =>
        {
            e.HasOne(x => x.Composite).WithMany(x => x.Excerpts);
            e.HasOne(x => x.Element).WithMany(x => x.Excerpts);
        });

        ConfigureEntity<UnitOfMeasure>(modelBuilder, (e) =>
        {
            e.HasIndex(x => x.Name).IsUnique();
            e.Property(x => x.Name).IsRequired();
            e.Property(x => x.Symbol).IsRequired();
        });

        ConfigureEntity<AppUser>(modelBuilder, (e) =>
        {
            e.HasIndex(x => x.Name).IsUnique();
            e.Property(x => x.Name).IsRequired();
        });

        base.OnModelCreating(modelBuilder);
    }

    private void ConfigureEntity<T>(ModelBuilder mb, Action<EntityTypeBuilder<T>>? customConfiguration = null) where T : EntityBase
    {
        var name = typeof(T).Name;
        mb.Entity<T>(e =>
        {
            e.ToTable(name);
            e.HasKey(x => x.Id);
            customConfiguration?.Invoke(e);
            e.HasQueryFilter(x => x.EntityStatus != eEntityStatus.Deleted);
        });
    }
}

