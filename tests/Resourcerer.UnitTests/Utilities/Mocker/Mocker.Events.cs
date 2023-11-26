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

    public static ItemCancelledEvent MockOrderCancelledEvent(
        AppDbContext context,
        ItemOrderedEvent boughtEvent,
        Action<ItemCancelledEvent>? modifier = null)
    {
        var entity = MakeEntity(() => new ItemCancelledEvent
        {
            InstanceBoughtEvent = boughtEvent
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

    public static ItemSoldEvent MockSoldEvent(
        AppDbContext context,
        ItemOrderedEvent orderedEvent,
        Action<ItemSoldEvent>? modifier = null)
    {
        var entity = MakeEntity(() => new ItemSoldEvent
        {
            TotalDiscountPercent = 0,
            UnitPrice = 1,
            Quantity = 1,
        });

        modifier?.Invoke(entity);

        orderedEvent.Instance!.ItemSoldEvents.Add(entity);

        context.ItemSoldEvents.Add(entity);

        return entity;
    }

    public static ItemCancelledEvent MockSellCancelledEvent(
        AppDbContext context,
        ItemSoldEvent soldEvent,
        Action<ItemCancelledEvent>? modifier = null)
    {
        var entity = MakeEntity(() => new ItemCancelledEvent
        {
            InstanceSoldEvent = soldEvent
        });

        modifier?.Invoke(entity);

        context.ItemCancelledEvents.Add(entity);

        return entity;
    }
}
