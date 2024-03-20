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
        ConfigureEntity<Company>(modelBuilder, e =>
        {
            e.ToTable("Companies");
            e.Property(x => x.Name).IsRequired();
        });

        ConfigureEntity<AppUser>(modelBuilder, (e) =>
        {
            e.HasIndex(x => x.Name).IsUnique();
            e.Property(x => x.Name).IsRequired();

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

        ConfigureEntity<Excerpt>(modelBuilder, (e) =>
        {
            e.HasKey(x => new { x.CompositeId, x.ElementId });

            e.HasOne(x => x.Composite).WithMany(x => x.CompositeExcerpts)
                .HasForeignKey(x => x.CompositeId).IsRequired()
                .HasConstraintName($"FK_Composite{nameof(Item)}_{nameof(Excerpt)}");

            e.HasOne(x => x.Element).WithMany(x => x.ElementExcerpts)
                .HasForeignKey(x => x.ElementId).IsRequired()
                .HasConstraintName($"FK_Element{nameof(Item)}_{nameof(Excerpt)}");
        }, customKey: true);

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
        });


        ConfigureEntity<InstanceReservedEvent>(modelBuilder, (e) =>
        {
            e.HasOne(x => x.Instance).WithMany(x => x.ReservedEvents)
                .HasForeignKey(x => x.InstanceId)
                .HasConstraintName($"FK_{nameof(Instance)}_{nameof(InstanceReservedEvent)}");
        });
        
        ConfigureEntity<InstanceDiscardedEvent>(modelBuilder, (e) =>
        {
            e.HasOne(x => x.Instance).WithMany(x => x.DiscardedEvents)
                .HasForeignKey(x => x.InstanceId)
                .HasConstraintName($"FK_{nameof(Instance)}_{nameof(InstanceDiscardedEvent)}");
        });

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if(typeof(AppDbEntity).IsAssignableFrom(entityType.ClrType))
            {
                var param = Expression.Parameter(entityType.ClrType, "i");
                var prop = Expression.PropertyOrField(param, nameof(AppDbEntity.EntityStatus));
                var expression = Expression.NotEqual(prop, Expression.Constant(eEntityStatus.Deleted));

                entityType.SetQueryFilter(Expression.Lambda(expression, param));
            }
        }

        base.OnModelCreating(modelBuilder);
    }

    private void ConfigureEntity<T>(ModelBuilder mb, Action<EntityTypeBuilder<T>>? customConfiguration = null, bool customKey = false) where T : AppDbEntity
    {
        var name = typeof(T).Name;
        mb.Entity<T>(e =>
        {
            e.ToTable(name);
            if(!customKey)
            {
                e.HasKey(x => x.Id);
            }
            e.HasQueryFilter(x => x.EntityStatus == eEntityStatus.Active);
            customConfiguration?.Invoke(e);
        });
    }
}

