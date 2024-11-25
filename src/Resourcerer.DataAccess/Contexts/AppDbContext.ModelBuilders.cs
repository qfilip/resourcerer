using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Resourcerer.DataAccess.Abstractions;
using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Enums;
using Resourcerer.DataAccess.Records;
using System.Linq.Expressions;

namespace Resourcerer.DataAccess.Contexts;
public partial class AppDbContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureEntity<Company>(modelBuilder, e =>
        {
            e.ToTable("Companies");
            e.HasIndex(x => x.Name).IsUnique();
            e.Property(x => x.Name).IsRequired();
        });

        ConfigureEntity<AppUser>(modelBuilder, (e) =>
        {
            e.Property(x => x.Name).IsRequired();
            e.HasIndex(x => x.Email).IsUnique();

            e.HasOne(x => x.Company).WithMany(x => x.Employees)
                .HasForeignKey(x => x.CompanyId)
                .HasConstraintName($"FK_{nameof(Company)}_{nameof(AppUser)}");
        });

        ConfigureEntity<Category>(modelBuilder, e =>
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

        ConfigureEntity<Recipe>(modelBuilder, (e) =>
        {
            e.HasOne(x => x.CompositeItem).WithMany(x => x.Recipes)
                .HasForeignKey(x => x.CompositeItemId)
                .IsRequired(true)
                .HasConstraintName($"FK_{nameof(Item)}_{nameof(Recipe)}");

            e.HasIndex(x => new { x.CompositeItemId, x.Version })
                .IsUnique();
        });

        ConfigureEntity<RecipeExcerpt>(modelBuilder, (e) =>
        {
            e.HasKey(x => new { x.RecipeId, x.ElementId });

            e.HasOne(x => x.Recipe).WithMany(x => x.RecipeExcerpts)
                .HasForeignKey(x => x.RecipeId)
                .IsRequired()
                .HasConstraintName($"FK_Composite{nameof(Recipe)}_{nameof(RecipeExcerpt)}");

            e.HasOne(x => x.Element).WithMany(x => x.ElementRecipeExcerpts)
                .HasForeignKey(x => x.ElementId)
                .IsRequired()
                .HasConstraintName($"FK_Element{nameof(Item)}_{nameof(RecipeExcerpt)}");
        });

        ConfigureEntity<UnitOfMeasure>(modelBuilder, (e) =>
        {
            e.HasIndex(x => x.Name).IsUnique();
            e.Property(x => x.Name).IsRequired();
            e.Property(x => x.Symbol).IsRequired();

            e.HasOne(x => x.Company).WithMany(x => x.UnitsOfMeasure)
                .HasForeignKey(x => x.CompanyId)
                .HasConstraintName($"FK_Element{nameof(Company)}_{nameof(UnitOfMeasure)}");
        });

        ConfigureEntity<Price>(modelBuilder, (e) =>
        {
            e.Property(x => x.UnitValue).IsRequired();

            e.HasOne(x => x.Item).WithMany(x => x.Prices)
                .HasForeignKey(x => x.ItemId)
                .HasConstraintName($"FK_{nameof(Item)}_{nameof(Price)}");
        });

        ConfigureEntity<Item>(modelBuilder, e =>
        {
            e.Property(x => x.Name).IsRequired();

            e.HasOne(x => x.Category).WithMany(x => x.Items)
                .HasForeignKey(x => x.CategoryId).IsRequired()
                .HasConstraintName($"FK_{nameof(Category)}_{nameof(Item)}");

            e.HasOne(x => x.UnitOfMeasure).WithMany(x => x.Items)
                .HasForeignKey(x => x.UnitOfMeasureId).IsRequired()
                .HasConstraintName($"FK_{nameof(UnitOfMeasure)}_{nameof(Item)}");
        });

        ConfigureEntity<ItemProductionOrder>(modelBuilder, e =>
        {
            e.HasOne(x => x.Item).WithMany(x => x.ProductionOrders)
                .HasForeignKey(x => x.ItemId).IsRequired()
                .HasConstraintName($"FK_{nameof(Item)}_{nameof(ItemProductionOrder)}");

            e.OwnsOne(x => x.StartedEvent, nav =>
            {
                nav.OwnsOne(n => n.AuditRecord, nb => nb.ToJson());
                nav.ToJson();
            });
            e.OwnsOne(x => x.CancelledEvent, nav =>
            {
                nav.OwnsOne(n => n.AuditRecord, nb => nb.ToJson());
                nav.ToJson();
            });
            e.OwnsOne(x => x.FinishedEvent, nav =>
            {
                nav.OwnsOne(n => n.AuditRecord, nb => nb.ToJson());
                nav.ToJson();
            });
        });

        ConfigureEntity<Instance>(modelBuilder, (e) =>
        {
            e.HasOne(x => x.Item).WithMany(x => x.Instances)
                .HasForeignKey(x => x.ItemId)
                .HasConstraintName($"FK_{nameof(Item)}_{nameof(Instance)}");

            e.HasOne(x => x.OwnerCompany).WithMany(x => x.Instances)
                .HasForeignKey(x => x.OwnerCompanyId)
                .HasConstraintName($"FK_{nameof(Company)}_{nameof(Instance)}");

            e.HasOne(x => x.SourceInstance).WithMany(x => x.DerivedInstances)
                .HasForeignKey(x => x.SourceInstanceId)
                .HasConstraintName($"FK_{nameof(Instance)}_{nameof(Instance)}");
        });

        ConfigureEntity<InstanceOrderedEvent>(modelBuilder, (e) =>
        {
            e.HasOne(x => x.Instance).WithMany(x => x.OrderedEvents)
                .HasForeignKey(x => x.InstanceId)
                .HasConstraintName($"FK_{nameof(Instance)}_{nameof(InstanceOrderedEvent)}");

            e.OwnsOne(x => x.CancelledEvent, nav =>
            {
                nav.OwnsOne(n => n.AuditRecord, nb => nb.ToJson());
                nav.ToJson();
            });
            e.OwnsOne(x => x.SentEvent, nav =>
            {
                nav.OwnsOne(n => n.AuditRecord, nb => nb.ToJson());
                nav.ToJson();
            });
            e.OwnsOne(x => x.DeliveredEvent, nav =>
            {
                nav.OwnsOne(n => n.AuditRecord, nb => nb.ToJson());
                nav.ToJson();
            });
        });


        ConfigureEntity<InstanceReservedEvent>(modelBuilder, (e) =>
        {
            e.HasOne(x => x.Instance).WithMany(x => x.ReservedEvents)
                .HasForeignKey(x => x.InstanceId)
                .HasConstraintName($"FK_{nameof(Instance)}_{nameof(InstanceReservedEvent)}");

            e.OwnsOne(x => x.CancelledEvent, nav =>
            {
                nav.OwnsOne(n => n.AuditRecord, nb => nb.ToJson());
                nav.ToJson();
            });
            e.OwnsOne(x => x.UsedEvent, nav =>
            {
                nav.OwnsOne(n => n.AuditRecord, nb => nb.ToJson());
                nav.ToJson();
            });
        });
        
        ConfigureEntity<InstanceDiscardedEvent>(modelBuilder, (e) =>
        {
            e.HasOne(x => x.Instance).WithMany(x => x.DiscardedEvents)
                .HasForeignKey(x => x.InstanceId)
                .HasConstraintName($"FK_{nameof(Instance)}_{nameof(InstanceDiscardedEvent)}");
        });

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if(entityType.ClrType.IsAssignableFrom(typeof(ISoftDeletable)))
            {
                var param = Expression.Parameter(entityType.ClrType, "i");
                var prop = Expression.PropertyOrField(param, nameof(ISoftDeletable.EntityStatus));
                var expression = Expression.NotEqual(prop, Expression.Constant(eEntityStatus.Deleted));

                entityType.SetQueryFilter(Expression.Lambda(expression, param));
            }
        }

        base.OnModelCreating(modelBuilder);
    }

    private void ConfigureEntity<TEntity>(
        ModelBuilder mb,
        Action<EntityTypeBuilder<TEntity>>? customConfiguration = null)
        where TEntity : class
    {
        ConfigureEntity<TEntity, Guid, Audit>(mb, customConfiguration);
    }

    private void ConfigureEntity<TEntity, TPKey, TAudit>(
        ModelBuilder mb,
        Action<EntityTypeBuilder<TEntity>>? customConfiguration = null)
        where TEntity : class
        where TPKey : struct
        where TAudit : class
    {
        var type = typeof(TEntity);
        mb.Entity<TEntity>(e =>
        {
            e.ToTable(type.Name);

            if (typeof(IId<TPKey>).IsAssignableFrom(type))
                e.HasKey(x => ((IId<TPKey>)x).Id);

            if (typeof(ISoftDeletable).IsAssignableFrom(type))
                e.HasQueryFilter(x => ((ISoftDeletable)x).EntityStatus == eEntityStatus.Active);

            if (typeof(IAuditedEntity<TAudit>).IsAssignableFrom(type))
                e.OwnsOne(x => ((IAuditedEntity<TAudit>)x).AuditRecord);

            customConfiguration?.Invoke(e);
        });
    }
}

