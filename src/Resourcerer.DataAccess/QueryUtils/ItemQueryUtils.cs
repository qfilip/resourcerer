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
                .ThenInclude(x => x.OrderedEvents)
                    .ThenInclude(x => x!.InstanceDeliveredEvent)
            
            // cancelations
            .Include(x => x.Instances)
                .ThenInclude(x => x.OrderedEvents)
                    .ThenInclude(x => x!.InstanceOrderCancelledEvent)

            // sent
            .Include(x => x.Instances)
                .ThenInclude(x => x.OrderedEvents)
                    .ThenInclude(x => x!.InstanceSentEvent)

            // discards
            .Include(x => x.Instances)
                .ThenInclude(x => x.DiscardedEvents);
    }
}
