using Resourcerer.DataAccess.Entities;

namespace Resourcerer.Logic.V1.Functions;

public static partial class Items
{
    public static double GetCompositeItemProductionCost(Item item)
    {
        return item.Recipes
            .OrderByDescending(x => x.Version)
            .First()
            .RecipeExcerpts
            .SelectMany(x => x.Element!.Prices)
            .Sum(x => x.UnitValue);
    }
}
