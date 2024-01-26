﻿using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Entities;

namespace Resourcerer.DataAccess.QueryUtils;

public static class ItemQueryUtils
{
    public static IQueryable<Item> IncludeInstanceEvents(IQueryable<Item> query)
    {
        return query
             // delivered
            .Include(x => x.Instances)
                .ThenInclude(x => x.ItemOrderedEvent)
                    .ThenInclude(x => x!.ItemDeliveredEvent)
            // cancelations
            .Include(x => x.Instances)
                .ThenInclude(x => x.ItemOrderedEvent)
                    .ThenInclude(x => x!.ItemOrderCancelledEvent)
            // sells
            .Include(x => x.Instances)
                .ThenInclude(x => x.ItemSoldEvents)
                    .ThenInclude(x => x!.ItemSellCancelledEvent)
            // discards
            .Include(x => x.Instances)
                .ThenInclude(x => x.ItemDiscardedEvents);
    }
}
