using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Entities;

namespace Resourcerer.DataAccess.QueryUtils;

public static class ItemQueryUtils
{
    public static IQueryable<Item> IncludeInstanceEvents(IQueryable<Item> query)
    {
        return query
             // delivered
            .Include(x => x.Instances)
                .ThenInclude(x => x.InstanceBoughtEvent)
                    .ThenInclude(x => x!.ItemDeliveredEvent)
            // sells
            .Include(x => x.Instances)
                .ThenInclude(x => x.InstanceSoldEvents)
                    .ThenInclude(x => x!.ItemSellCancelledEvent)
            // cancelations
            .Include(x => x.Instances)
                .ThenInclude(x => x.InstanceBoughtEvent)
                    .ThenInclude(x => x!.ItemSellCancelledEvent)
            // discards
            .Include(x => x.Instances)
                .ThenInclude(x => x.InstanceDiscardedEvents);
    }
}
