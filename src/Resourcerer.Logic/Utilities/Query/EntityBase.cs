using Resourcerer.DataAccess.Abstractions;
using Resourcerer.DataAccess.Records;
using Resourcerer.Dtos.Entities;
using System.Linq.Expressions;

namespace Resourcerer.Logic.Utilities.Query;

internal static class EntityBases
{
    public static Expression<Func<T, T>> GetDefaultProjection<T>() where T : IId<Guid>, IAuditedEntity<Audit>, ISoftDeletable, new()
    {
        Expression<Func<T, T>> exp = (x) => new T
        {
            Id = x.Id,
            EntityStatus = x.EntityStatus,
            AuditRecord = new()
            {
                CreatedAt = x.AuditRecord.CreatedAt,
                CreatedBy = x.AuditRecord.CreatedBy,
                ModifiedAt = x.AuditRecord.ModifiedAt,
                ModifiedBy = x.AuditRecord.ModifiedBy
            }
        };

        return exp;
    }

    public static Expression<Func<T, T>> Expand<T>(Expression<Func<T, T>> selector) where T : IId<Guid>, IAuditedEntity<Audit>, ISoftDeletable, new()
    {
        return ExpressionUtils.Combine(GetDefaultProjection<T>(), selector);
    }

    public static Expression<Func<T, TDto>> GetDefaultCustomProjection<T, TDto>()
        where T : IId<Guid>, IAuditedEntity<Audit>, ISoftDeletable, new()
        where TDto : EntityDto, new()
    {
        Expression<Func<T, TDto>> exp = (x) => new TDto
        {
            Id = x.Id,
            EntityStatus = x.EntityStatus,
            AuditRecord = new()
            {
                CreatedAt = x.AuditRecord.CreatedAt,
                CreatedBy = x.AuditRecord.CreatedBy,
                ModifiedAt = x.AuditRecord.ModifiedAt,
                ModifiedBy = x.AuditRecord.ModifiedBy
            }
        };

        return exp;
    }

    public static Expression<Func<T, TDto>> Expand<T, TDto>(Expression<Func<T, TDto>> selector)
        where T : IId<Guid>, IAuditedEntity<Audit>, ISoftDeletable, new()
        where TDto: EntityDto, new()
    {
        return ExpressionUtils.CombineDto(GetDefaultCustomProjection<T, TDto>(), selector);
    }
}
