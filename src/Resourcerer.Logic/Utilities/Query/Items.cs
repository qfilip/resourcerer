using Resourcerer.DataAccess.Entities;
using System.Linq.Expressions;

namespace Resourcerer.Logic.Utilities.Query;

public static class Items
{
    public static Expression<Func<Item, Item>> DefaultProjection =
        EntityBases.Expand<Item>(x => new Item
        {
            Name = x.Name,
            ProductionPrice = x.ProductionPrice,
            ProductionTimeSeconds = x.ProductionTimeSeconds,
            ExpirationTimeSeconds = x.ExpirationTimeSeconds,
            
            CategoryId = x.CategoryId,
            UnitOfMeasureId = x.UnitOfMeasureId
        });

    public static Expression<Func<Item, Item>> Expand(Expression<Func<Item, Item>> projectionLayer)
    {
        return ExpressionUtils.Combine(DefaultProjection, projectionLayer);
    }
}
