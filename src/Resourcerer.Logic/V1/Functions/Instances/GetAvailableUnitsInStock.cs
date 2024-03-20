using Resourcerer.DataAccess.Entities;

namespace Resourcerer.Logic.V1_0.Functions;

public static partial class Instances
{
    public static double GetAvailableUnitsInStock(Instance i)
    {
        var inStock = GetUnitsInStock(i);

        var reserved = i.ReservedEvents
            .Where(x => x.CancelledEvent == null)
            .Sum(x => x.Quantity);

        return inStock - reserved;
    }
}
