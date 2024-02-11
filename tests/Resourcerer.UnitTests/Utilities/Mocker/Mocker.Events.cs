using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;

namespace Resourcerer.UnitTests.Utilities.Mocker;

internal static partial class Mocker
{
    public static InstanceOrderedEvent MockOrderedEvent(
        AppDbContext context,
        Item item,
        Action<InstanceOrderedEvent>? modifier = null)
    {
        var entity = MakeEntity(() => new InstanceOrderedEvent
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

    public static InstanceOrderCancelledEvent MockOrderCancelledEvent(
        AppDbContext context,
        InstanceOrderedEvent boughtEvent,
        Action<InstanceOrderCancelledEvent>? modifier = null)
    {
        var entity = MakeEntity(() => new InstanceOrderCancelledEvent
        {
            ItemOrderedEvent = boughtEvent
        });

        modifier?.Invoke(entity);

        context.ItemCancelledEvents.Add(entity);

        return entity;
    }

    public static InstanceDeliveredEvent MockDeliveredEvent(
        AppDbContext context,
        InstanceOrderedEvent orderEvent,
        Action<InstanceDeliveredEvent>? modifier = null)
    {
        var entity = MakeEntity(() => new InstanceDeliveredEvent
        {
            ItemOrderedEvent = orderEvent
        });

        modifier?.Invoke(entity);

        context.ItemDeliveredEvents.Add(entity);

        return entity;
    }

    public static InstanceDiscardedEvent MockDiscardedEvent(
        AppDbContext context,
        InstanceOrderedEvent orderEvent,
        Action<InstanceDiscardedEvent>? modifier = null)
    {
        var entity = MakeEntity(() => new InstanceDiscardedEvent
        {
            Instance = orderEvent.Instance,
            Quantity = 1,
            Reason = "test reason"
        });

        modifier?.Invoke(entity);

        context.ItemDiscardedEvents.Add(entity);

        return entity;
    }

    public static InstanceOrderCancelledEvent MockSellCancelledEvent(
        AppDbContext context,
        InstanceOrderedEvent orderEvent,
        Action<InstanceOrderCancelledEvent>? modifier = null)
    {
        var entity = MakeEntity(() => new InstanceOrderCancelledEvent
        {
            ItemOrderedEvent = orderEvent
        });

        modifier?.Invoke(entity);

        context.ItemCancelledEvents.Add(entity);

        return entity;
    }
}
