﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Enums;
using System.Linq.Expressions;

namespace Resourcerer.DataAccess.Contexts;
public partial class AppDbContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureEntity<Category>(modelBuilder, e =>
        {
            e.Property(x => x.Name).IsRequired();
            
            e.HasOne(x => x.ParentCategory).WithMany(x => x.ChildCategories)
                .HasForeignKey(x => x.ParentCategoryId)
                .HasConstraintName($"FK_{nameof(Category)}_{nameof(Category)}");
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

        ConfigureEntity<CompositeSoldEvent>(modelBuilder, e =>
        {
            e.HasOne(x => x.Composite).WithMany(x => x.CompositeSoldEvents)
                .HasForeignKey(x => x.CompositeId).IsRequired()
                .HasConstraintName($"FK_{nameof(Composite)}_{nameof(CompositeSoldEvent)}");
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

        ConfigureEntity<ElementPurchasedEvent>(modelBuilder, e =>
        {
            e.HasOne(x => x.Element).WithMany(x => x.ElementPurchasedEvents)
                .HasForeignKey(x => x.ElementId).IsRequired()
                .HasConstraintName($"FK_{nameof(Element)}_{nameof(ElementPurchasedEvent)}");
        });

        ConfigureEntity<ElementPurchaseCancelledEvent>(modelBuilder, e =>
        {
            e.HasOne(x => x.ElementPurchasedEvent).WithOne(x => x.ElementPurchaseCancelledEvent)
                .HasForeignKey<ElementPurchaseCancelledEvent>(x => x.ElementPurchasedEventId)
                .IsRequired()
                .HasConstraintName($"FK_{nameof(ElementPurchaseCancelledEvent)}_{nameof(ElementPurchaseCancelledEvent)}");
        });

        ConfigureEntity<ElementDeliveredEvent>(modelBuilder, e =>
        {
            e.HasOne(x => x.ElementPurchasedEvent).WithOne(x => x.ElementDeliveredEvent)
                .HasForeignKey<ElementDeliveredEvent>(x => x.ElementPurchasedEventId)
                .IsRequired()
                .HasConstraintName($"FK_{nameof(ElementPurchasedEvent)}_{nameof(ElementDeliveredEvent)}");
        });

        ConfigureEntity<ElementInstanceDiscardedEvent>(modelBuilder, e =>
        {
            e.HasOne(x => x.ElementInstance).WithMany(x => x.ElementInstanceDiscardedEvents)
                .HasForeignKey(x => x.ElementInstanceId).IsRequired()
                .HasConstraintName($"FK_{nameof(Instance)}_{nameof(ElementInstanceDiscardedEvent)}");
        });

        ConfigureEntity<ElementInstanceSoldEvent>(modelBuilder, e =>
        {
            e.HasOne(x => x.ElementInstance).WithMany(x => x.ElementInstanceSoldEvents)
                .HasForeignKey(x => x.ElementInstanceId).IsRequired()
                .HasConstraintName($"FK_{nameof(Instance)}_{nameof(ElementInstanceSoldEvent)}");
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

        ConfigureEntity<Instance>(modelBuilder, (e) =>
        {
            e.Property(x => x.Quantity).IsRequired();

            e.HasOne(x => x.Element).WithMany(x => x.ElementInstances)
                .HasForeignKey(x => x.ElementId)
                .HasConstraintName($"FK_{nameof(Element)}_{nameof(Instance)}");

            e.HasOne(x => x.Composite).WithMany(x => x.CompositeInstances)
                .HasForeignKey(x => x.CompositeId)
                .HasConstraintName($"FK_{nameof(Composite)}_{nameof(Instance)}");
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

        ConfigureEntity<AppUser>(modelBuilder, (e) =>
        {
            e.HasIndex(x => x.Name).IsUnique();
            e.Property(x => x.Name).IsRequired();
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

