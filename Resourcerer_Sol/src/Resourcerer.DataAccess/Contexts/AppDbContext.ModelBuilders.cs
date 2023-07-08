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
            
            e.HasOne(x => x.ParentCategory).WithMany(x => x.ChildCategories)
                .HasForeignKey(x => x.ParentCategoryId);
        });

        ConfigureEntity<Composite>(modelBuilder, e =>
        {
            e.HasIndex(x => x.Name).IsUnique();
            e.Property(x => x.Name).IsRequired();
            
            e.HasOne(x => x.Category).WithMany(x => x.Composites)
                .HasForeignKey(x => x.CategoryId).IsRequired();
            
            e.HasOne(x => x.UnitOfMeasure).WithMany(x => x.Composites)
                .HasForeignKey(x => x.UnitOfMeasureId).IsRequired();
        });

        ConfigureEntity<CompositeSoldEvent>(modelBuilder, e =>
        {
            e.HasOne(x => x.Composite).WithMany(x => x.CompositeSoldEvents)
                .HasForeignKey(x => x.CompositeId).IsRequired();
        });

        ConfigureEntity<Element>(modelBuilder, e =>
        {
            e.HasIndex(x => x.Name).IsUnique();
            e.Property(x => x.Name).IsRequired();
            
            e.HasOne(x => x.Category).WithMany(x => x.Elements)
                .HasForeignKey(x => x.CategoryId).IsRequired();
            
            e.HasOne(x => x.UnitOfMeasure).WithMany(x => x.Elements)
                .HasForeignKey(x => x.UnitOfMeasureId).IsRequired();
        });

        ConfigureEntity<ElementPurchasedEvent>(modelBuilder, e =>
        {
            e.HasOne(x => x.Element).WithMany(x => x.ElementPurchasedEvents)
                .HasForeignKey(x => x.ElementId).IsRequired();
        });

        ConfigureEntity<ElementPurchaseCancelledEvent>(modelBuilder, e =>
        {
            e.HasOne(x => x.ElementPurchasedEvent).WithOne(x => x.ElementPurchaseCancelledEvent)
                .HasForeignKey<ElementPurchaseCancelledEvent>(x => x.ElementPurchasedEventId)
                .IsRequired();
        });

        ConfigureEntity<ElementDeliveredEvent>(modelBuilder, e =>
        {
            e.HasOne(x => x.ElementPurchasedEvent).WithOne(x => x.ElementDeliveredEvent)
                .HasForeignKey<ElementDeliveredEvent>(x => x.ElementPurchasedEventId)
                .IsRequired();
        });

        ConfigureEntity<ElementInstanceDiscardedEvent>(modelBuilder, e =>
        {
            e.HasOne(x => x.ElementInstance).WithMany(x => x.ElementInstanceDiscardedEvents)
                .HasForeignKey(x => x.ElementInstanceId).IsRequired();
        });

        ConfigureEntity<ElementInstanceSoldEvent>(modelBuilder, e =>
        {
            e.HasOne(x => x.ElementInstance).WithMany(x => x.ElementInstanceSoldEvents)
                .HasForeignKey(x => x.ElementInstanceId).IsRequired();
        });

        ConfigureEntity<Price>(modelBuilder, (e) =>
        {
            e.Property(x => x.UnitValue).IsRequired();
            
            e.HasOne(x => x.Element).WithMany(x => x.Prices)
                .HasForeignKey(x => x.ElementId);

            e.HasOne(x => x.Composite).WithMany(x => x.Prices)
                .HasForeignKey(x => x.CompositeId);
        });

        ConfigureEntity<Instance>(modelBuilder, (e) =>
        {
            e.Property(x => x.Quantity).IsRequired();

            e.HasOne(x => x.Element).WithMany(x => x.ElementInstances)
                .HasForeignKey(x => x.ElementId);

            e.HasOne(x => x.Composite).WithMany(x => x.CompositeInstances)
                .HasForeignKey(x => x.CompositeId);
        });

        ConfigureEntity<Excerpt>(modelBuilder, (e) =>
        {
            e.HasOne(x => x.Composite).WithMany(x => x.Excerpts)
                .HasForeignKey(x => x.CompositeId).IsRequired();

            e.HasOne(x => x.Element).WithMany(x => x.Excerpts)
                .HasForeignKey(x => x.ElementId).IsRequired();
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
            e.HasQueryFilter(x => x.EntityStatus == eEntityStatus.Active);
        });
    }
}

