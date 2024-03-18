using Resourcerer.DataAccess.Entities;
using System.Linq.Expressions;

namespace Resourcerer.DataAccess.Utilities.Query;

internal static class EntityBases
{
    public static Expression<Func<T, T>> GetDefaultProjection<T>() where T : EntityBase, new()
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

    public static Expression<Func<T, T>> Expand<T>(Expression<Func<T, T>> selector) where T : EntityBase, new()
    {
        return ExpressionUtils.Combine(GetDefaultProjection<T>(), selector);
    }
}
