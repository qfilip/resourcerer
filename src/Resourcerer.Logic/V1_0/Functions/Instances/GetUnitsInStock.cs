using Resourcerer.DataAccess.Entities;

namespace Resourcerer.Logic.V1_0.Functions;

public static partial class Instances
{
    public static double GetUnitsInStock(Instance i)
    {
        if (i.SourceInstanceId == null)
        {
            var sent = i.OrderedEvents
                .Where(x => x.SentEvent == null)
                .Sum(x => x.Quantity);

            return i.Quantity - sent;
        }

        if (i.SourceInstance == null)
        {
            throw new InvalidOperationException($"Source instance for instance {i.Id} is null");
        }

        var sourceInstancesDelivered = i.SourceInstance.OrderedEvents
            .FirstOrDefault(x =>
                x.DerivedInstanceId == i.Id &&
                x.DeliveredEvent != null)?.Quantity ?? 0;

        var instancesSent = i.OrderedEvents
            .Where(x => x.SentEvent != null)
            .Sum(x => x.Quantity);

        return sourceInstancesDelivered - instancesSent;
    }
}
