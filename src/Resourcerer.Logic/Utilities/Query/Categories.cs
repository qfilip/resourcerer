using Resourcerer.DataAccess.Entities;
using System.Linq.Expressions;

namespace Resourcerer.Logic.Utilities.Query;

internal static class Categories
{
    internal static Expression<Func<Category, Category>> DefaultProjection =
        EntityBases.Expand<Category>(x => new Category
        {
            Id = x.Id,
            Name = x.Name,
            CompanyId = x.CompanyId,
            ParentCategoryId = x.ParentCategoryId
        });

    internal static Expression<Func<Category, Category>> Expand(Expression<Func<Category, Category>> projectionLayer)
    {
        return ExpressionUtils.Combine(DefaultProjection, projectionLayer);
    }
}
