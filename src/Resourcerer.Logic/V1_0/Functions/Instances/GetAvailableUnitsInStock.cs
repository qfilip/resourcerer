using Resourcerer.DataAccess.Entities;

namespace Resourcerer.Logic.V1_0.Functions;

public static partial class Instances
{
    // not marked for sending (not reserved units)
    public static double GetAvailableUnitsInStock(Instance i)
    {
        var unitsInStock = GetUnitsInStock(i);
        var reservedQuantity = i.ReservedEvents.Sum(e => e.Quantity);
        var markedForSending = i.OrderedEvents
            .Where(x =>
                x.OrderCancelledEvent == null &&
                x.SentEvent == null)
            .Sum(x => x.Quantity);

        return unitsInStock - reservedQuantity - markedForSending;
    }
}
