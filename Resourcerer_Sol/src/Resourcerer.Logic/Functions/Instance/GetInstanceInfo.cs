using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos;

namespace Resourcerer.Logic.Functions;

public static partial class Instances
{
    public static InstanceInfoDto? GetInstanceStockInfo(Instance i)
    {
        if (i.InstanceBuyRequestedEvent!.InstanceRequestDeliveredEvent == null)
        {
            return null;
        }

        var sold = i.InstanceSellRequestedEvents
            .Where(x => x.InstanceRequestCancelledEvent == null)
            .Sum(x => x.UnitsSold);

        var discarded = i.InstanceDiscardedEvents
            .Sum(x => x.Quantity);
    }
}
