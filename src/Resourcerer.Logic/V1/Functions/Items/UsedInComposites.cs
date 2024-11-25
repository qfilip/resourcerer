using Resourcerer.DataAccess.Entities;

namespace Resourcerer.Logic.V1.Functions;

public static partial class Items
{
    public static IEnumerable<Item> UsedInComposites(Item item)
    {
        return item.ElementRecipeExcerpts
            .Select(x => x.Recipe!.CompositeItem!);
    }
}
