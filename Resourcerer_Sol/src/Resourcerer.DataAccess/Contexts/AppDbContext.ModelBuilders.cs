using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Enums;
using System.Linq.Expressions;

namespace Resourcerer.DataAccess.Contexts;
public partial class AppDbContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureEntity<AppUser>(modelBuilder, (e) =>
        {
            e.HasIndex(x => x.Name).IsUnique();
            e.Property(x => x.Name).IsRequired();
        });

        ConfigureEntity<Category>(modelBuilder, e =>
        {
            e.Property(x => x.Name).IsRequired();
            
            e.HasOne(x => x.ParentCategory).WithMany(x => x.ChildCategories)
                .HasForeignKey(x => x.ParentCategoryId)
                .HasConstraintName($"FK_{nameof(Category)}_{nameof(Category)}");
        });

        ConfigureEntity<Excerpt>(modelBuilder, (e) =>
        {
            e.HasOne(x => x.Composite).WithMany(x => x.Excerpts)
                .HasForeignKey(x => x.CompositeId).IsRequired()
                .HasConstraintName($"FK_{nameof(Composite)}_{nameof(Excerpt)}");

            e.HasOne(x => x.Element).WithMany(x => x.Excerpts)
                .HasForeignKey(x => x.ElementId).IsRequired();
        });

        ConfigureEntity<UnitOfMeasure>(modelBuilder, (e) =>
        {
            e.HasIndex(x => x.Name).IsUnique();
            e.Property(x => x.Name).IsRequired();
            e.Property(x => x.Symbol).IsRequired();
        });

        ConfigureEntity<Price>(modelBuilder, (e) =>
        {
            e.Property(x => x.UnitValue).IsRequired();

            e.HasOne(x => x.Element).WithMany(x => x.Prices)
                .HasForeignKey(x => x.ElementId)
                .HasConstraintName($"FK_{nameof(Element)}_{nameof(Price)}");

            e.HasOne(x => x.Composite).WithMany(x => x.Prices)
                .HasForeignKey(x => x.CompositeId)
                .HasConstraintName($"FK_{nameof(Composite)}_{nameof(Price)}");
        });

        ConfigureEntity<Behavior>(modelBuilder, e =>
        {
            e.HasOne(x => x.Element).WithOne(x => x.Behavior)
                .HasForeignKey<Behavior>(x => x.ElementId)
                .HasConstraintName($"FK_{nameof(Element)}_{nameof(Behavior)}");

            e.HasOne(x => x.Composite).WithOne(x => x.Behavior)
                .HasForeignKey<Behavior>(x => x.CompositeId)
                .HasConstraintName($"FK_{nameof(Composite)}_{nameof(Behavior)}");
        });

        ConfigureEntity<Element>(modelBuilder, e =>
        {
            e.Property(x => x.Name).IsRequired();

            e.HasOne(x => x.Category).WithMany(x => x.Elements)
                .HasForeignKey(x => x.CategoryId).IsRequired()
                .HasConstraintName($"FK_{nameof(Category)}_{nameof(Element)}");

            e.HasOne(x => x.UnitOfMeasure).WithMany(x => x.Elements)
                .HasForeignKey(x => x.UnitOfMeasureId).IsRequired()
                .HasConstraintName($"FK_{nameof(UnitOfMeasure)}_{nameof(Element)}");
        });

        ConfigureEntity<Composite>(modelBuilder, e =>
        {
            e.Property(x => x.Name).IsRequired();
            
            e.HasOne(x => x.Category).WithMany(x => x.Composites)
                .HasForeignKey(x => x.CategoryId).IsRequired()
                .HasConstraintName($"FK_{nameof(Composite.Category)}_{nameof(Composite)}");

            e.HasOne(x => x.UnitOfMeasure).WithMany(x => x.Composites)
                .HasForeignKey(x => x.UnitOfMeasureId).IsRequired()
                .HasConstraintName($"FK_{nameof(Composite.UnitOfMeasure)}_{nameof(Composite)}");
        });

        ConfigureEntity<Instance>(modelBuilder, (e) =>
        {
            e.Property(x => x.Quantity).IsRequired();

            e.HasOne(x => x.Element).WithMany(x => x.Instances)
                .HasForeignKey(x => x.ElementId)
                .HasConstraintName($"FK_{nameof(Element)}_{nameof(Instance)}");

            e.HasOne(x => x.Composite).WithMany(x => x.Instances)
                .HasForeignKey(x => x.CompositeId)
                .HasConstraintName($"FK_{nameof(Composite)}_{nameof(Instance)}");
        });

        ConfigureEntity<InstanceOrderedEvent>(modelBuilder, (e) =>
        {
            e.HasOne(x => x.Instance).WithMany(x => x.InstanceOrderedEvents)
                .HasForeignKey(x => x.InstanceId)
                .IsRequired()
                .HasConstraintName($"FK_{nameof(Instance)}_{nameof(InstanceOrderedEvent)}");
        });

        ConfigureEntity<InstanceOrderCancelledEvent>(modelBuilder, (e) =>
        {
            e.HasOne(x => x.InstanceOrderedEvent).WithOne(x => x.InstanceOrderCancelledEvent)
                .HasForeignKey<InstanceOrderCancelledEvent>(x => x.InstanceOrderedEventId)
                .IsRequired()
                .HasConstraintName($"FK_{nameof(InstanceOrderedEvent)}_{nameof(InstanceOrderCancelledEvent)}");

            e.HasOne(x => x.InstanceDeliveredEvent).WithOne(x => x.InstanceOrderCancelledEvent)
                .HasForeignKey<InstanceOrderCancelledEvent>(x => x.InstanceDeliveredEventId)
                .HasConstraintName($"FK_{nameof(InstanceDeliveredEvent)}_{nameof(InstanceOrderCancelledEvent)}");
        });

        ConfigureEntity<InstanceDeliveredEvent>(modelBuilder, (e) =>
        {
            e.HasOne(x => x.InstanceOrderedEvent).WithOne(x => x.InstanceDeliveredEvent)
                .HasForeignKey<InstanceDeliveredEvent>(x => x.InstanceOrderedEventId)
                .IsRequired()
                .HasConstraintName($"FK_{nameof(InstanceOrderedEvent)}_{nameof(InstanceDeliveredEvent)}");

            e.HasOne(x => x.InstanceOrderCancelledEvent).WithOne(x => x.InstanceDeliveredEvent)
                .HasForeignKey<InstanceDeliveredEvent>(x => x.InstanceOrderCancelledEventId)
                .HasConstraintName($"FK_{nameof(InstanceOrderCancelledEvent)}_{nameof(InstanceDeliveredEvent)}");
        });

        ConfigureEntity<InstanceDiscardedEvent>(modelBuilder, e =>
        {
            e.HasOne(x => x.Instance).WithMany(x => x.InstanceDiscardedEvents)
                .HasForeignKey(x => x.InstanceId)
                .IsRequired()
                .HasConstraintName($"FK_{nameof(Instance)}_{nameof(InstanceDiscardedEvent)}");
        });

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if(typeof(EntityBase).IsAssignableFrom(entityType.ClrType))
            {
                var param = Expression.Parameter(entityType.ClrType, "i");
                var prop = Expression.PropertyOrField(param, nameof(EntityBase.EntityStatus));
                var expression = Expression.NotEqual(prop, Expression.Constant(eEntityStatus.Deleted));

                entityType.SetQueryFilter(Expression.Lambda(expression, param));
            }
        }

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

