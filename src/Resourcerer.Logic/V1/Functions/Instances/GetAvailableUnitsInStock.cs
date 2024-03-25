using Resourcerer.DataAccess.Entities;

namespace Resourcerer.Logic.V1.Functions;

public static partial class Instances
{
    public static double GetAvailableUnitsInStock(Instance i)
    {
        var ordered = i.OrderedEvents
            .Where(x => x.CancelledEvent == null)
            .Sum(x => x.Quantity);

        var reserved = i.ReservedEvents
            .Where(x => x.CancelledEvent == null)
            .Sum(x => x.Quantity);

        var discarded = i.DiscardedEvents.Sum(x => x.Quantity);

        var unavailable = ordered + reserved + discarded;

        // item produced
        if (i.SourceInstanceId == null)
        {
            return i.Quantity - unavailable;
        }

        // source instance exists, but not included
        if (i.SourceInstance == null)
        {
            throw new InvalidOperationException($"Source instance for instance {i.Id} is null");
        }

        var deliveredFromSource = i.SourceInstance.OrderedEvents
            .Where(x =>
                x.DerivedInstanceId == i.Id &&
                x.DeliveredEvent != null)
            .Sum(x => x.Quantity);

        return deliveredFromSource - unavailable;
    }
}
