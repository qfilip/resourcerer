using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Resourcerer.DataAccess.Abstractions;
using Resourcerer.DataAccess.Enums;
using Resourcerer.DataAccess.Records;
using System.Linq.Expressions;

namespace Resourcerer.DataAccess.Contexts;
public partial class AppDbContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

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

    internal static void ConfigureEntity<TEntity>(
        EntityTypeBuilder<TEntity> mb,
        Action<EntityTypeBuilder<TEntity>>? customConfiguration = null)
        where TEntity : class
    {
        ConfigureEntity<TEntity, Guid, Audit>(mb, customConfiguration);
    }

    internal static void ConfigureEntity<TEntity, TPKey, TAudit>(
        EntityTypeBuilder<TEntity> etb,
        Action<EntityTypeBuilder<TEntity>>? customConfiguration = null)
        where TEntity : class
        where TPKey : struct
        where TAudit : class
    {
        var type = typeof(TEntity);
        
        etb.ToTable(type.Name);

        if (typeof(IId<TPKey>).IsAssignableFrom(type))
            etb.HasKey(x => ((IId<TPKey>)x).Id);

        if (typeof(ISoftDeletable).IsAssignableFrom(type))
            etb.HasQueryFilter(x => ((ISoftDeletable)x).EntityStatus == eEntityStatus.Active);

        if (typeof(IAuditedEntity<TAudit>).IsAssignableFrom(type))
            etb.OwnsOne(x => ((IAuditedEntity<TAudit>)x).AuditRecord);

        customConfiguration?.Invoke(etb);
    }
}

