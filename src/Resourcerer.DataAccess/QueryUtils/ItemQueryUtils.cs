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
                .ThenInclude(x => x.ItemOrderedEvents)
                    .ThenInclude(x => x!.ItemDeliveredEvent)
            
            // cancelations
            .Include(x => x.Instances)
                .ThenInclude(x => x.ItemOrderedEvents)
                    .ThenInclude(x => x!.ItemOrderCancelledEvent)

            // sent
            .Include(x => x.Instances)
                .ThenInclude(x => x.ItemOrderedEvents)
                    .ThenInclude(x => x!.ItemSentEvent)

            // discards
            .Include(x => x.Instances)
                .ThenInclude(x => x.ItemDiscardedEvents);
    }
}
