using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;

namespace Resourcerer.UnitTests.Utilities.Mocker;

internal static partial class Mocker
{
    public static ItemOrderedEvent MockOrderedEvent(
        AppDbContext context,
        Item item,
        Action<ItemOrderedEvent>? modifier = null)
    {
        var entity = MakeEntity(() => new ItemOrderedEvent
        {
            ExpectedDeliveryDate = DateTime.UtcNow,
            TotalDiscountPercent = 0,
            UnitPrice = 1,
            Quantity = 1,

            Instance = MakeEntity(() => new Instance
            {
                ExpiryDate = DateTime.UtcNow,
            })
        });

        modifier?.Invoke(entity);

        entity.Instance!.Item = item;

        context.ItemOrderedEvents.Add(entity);
        
        return entity;
    }

    public static ItemOrderCancelledEvent MockOrderCancelledEvent(
        AppDbContext context,
        ItemOrderedEvent boughtEvent,
        Action<ItemOrderCancelledEvent>? modifier = null)
    {
        var entity = MakeEntity(() => new ItemOrderCancelledEvent
        {
            ItemOrderedEvent = boughtEvent
        });

        modifier?.Invoke(entity);

        context.ItemCancelledEvents.Add(entity);

        return entity;
    }

    public static ItemDeliveredEvent MockDeliveredEvent(
        AppDbContext context,
        ItemOrderedEvent orderEvent,
        Action<ItemDeliveredEvent>? modifier = null)
    {
        var entity = MakeEntity(() => new ItemDeliveredEvent
        {
            ItemOrderedEvent = orderEvent
        });

        modifier?.Invoke(entity);

        context.ItemDeliveredEvents.Add(entity);

        return entity;
    }

    public static ItemDiscardedEvent MockDiscardedEvent(
        AppDbContext context,
        ItemOrderedEvent orderEvent,
        Action<ItemDiscardedEvent>? modifier = null)
    {
        var entity = MakeEntity(() => new ItemDiscardedEvent
        {
            Instance = orderEvent.Instance,
            Quantity = 1,
            Reason = "test reason"
        });

        modifier?.Invoke(entity);

        context.ItemDiscardedEvents.Add(entity);

        return entity;
    }

    public static ItemOrderCancelledEvent MockSellCancelledEvent(
        AppDbContext context,
        ItemOrderedEvent orderEvent,
        Action<ItemOrderCancelledEvent>? modifier = null)
    {
        var entity = MakeEntity(() => new ItemOrderCancelledEvent
        {
            ItemOrderedEvent = orderEvent
        });

        modifier?.Invoke(entity);

        context.ItemCancelledEvents.Add(entity);

        return entity;
    }
}
