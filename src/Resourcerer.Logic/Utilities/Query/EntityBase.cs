using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.Entity;
using System.Linq.Expressions;

namespace Resourcerer.Logic.Utilities.Query;

internal static class EntityBases
{
    public static Expression<Func<T, T>> GetDefaultProjection<T>() where T : AppDbEntity, new()
    {
        Expression<Func<T, T>> exp = (x) => new T
        {
            Id = x.Id,
            EntityStatus = x.EntityStatus,
            CreatedAt = x.CreatedAt,
            CreatedBy = x.CreatedBy,
            ModifiedAt = x.ModifiedAt,
            ModifiedBy = x.ModifiedBy
        };

        return exp;
    }

    public static Expression<Func<T, T>> Expand<T>(Expression<Func<T, T>> selector) where T : AppDbEntity, new()
    {
        return ExpressionUtils.Combine(GetDefaultProjection<T>(), selector);
    }

    public static Expression<Func<T, TDto>> GetDefaultCustomProjection<T, TDto>()
        where T : AppDbEntity, new()
        where TDto : EntityDto<TDto>, new()
    {
        Expression<Func<T, TDto>> exp = (x) => new TDto
        {
            Id = x.Id,
            EntityStatus = x.EntityStatus,
            CreatedAt = x.CreatedAt,
            CreatedBy = x.CreatedBy,
            ModifiedAt = x.ModifiedAt,
            ModifiedBy = x.ModifiedBy
        };

        return exp;
    }

    public static Expression<Func<T, TDto>> Expand<T, TDto>(Expression<Func<T, TDto>> selector)
        where T : AppDbEntity, new()
        where TDto: EntityDto<TDto>, new()
    {
        return ExpressionUtils.CombineDto(GetDefaultCustomProjection<T, TDto>(), selector);
    }
}
