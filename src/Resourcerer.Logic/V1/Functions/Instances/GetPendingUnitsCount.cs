using Resourcerer.DataAccess.Entities;

namespace Resourcerer.Logic.V1.Functions;

public static partial class Instances
{
    /// <summary>
    /// Get ordered instances that aren't delivered yet.
    /// </summary>
    /// <param name="instances">Instances of a company that created the orders.</param>
    public static double GetPendingUnits(IEnumerable<Instance> instances)
    {   
        var incorrectCount = instances.Aggregate((0, 0), (acc, x) =>
        {
            if (x.SourceInstanceId == null)
                acc.Item1++;

            if (x.SourceInstance == null)
                acc.Item2++;

            return acc;
        });

        if (incorrectCount.Item1 > 0)
        {
            throw new InvalidOperationException($"No orders are available for produced instance.");
        }

        if (incorrectCount.Item2 > 0)
        {
            throw new InvalidOperationException($"Not all source instances are loaded.");
        }

        return instances
            .Sum(i =>
                i.SourceInstance!.OrderedEvents
                    .Where(x =>
                        x.DerivedInstanceId == i.Id &&
                        x.CancelledEvent == null &&
                        x.DeliveredEvent == null)
                    .Sum(x => x.Quantity));
    }
}
