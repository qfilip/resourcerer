using Resourcerer.DataAccess.Entities;

namespace Resourcerer.Logic.V1.Functions;

public static partial class Instances
{
    public static double GetAvailableUnitsInStock(Instance i)
    {
        var inStock = GetUnitsInStock(i);

        var reserved = i.ReservedEvents
            .Where(x => x.CancelledEvent == null)
            .Sum(x => x.Quantity);

        var discarded = i.DiscardedEvents.Sum(x => x.Quantity);

        return inStock - (reserved + discarded);
    }
}
