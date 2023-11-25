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

    public static ItemSellCancelledEvent MockOrderCancelledEvent(
        AppDbContext context,
        ItemOrderedEvent boughtEvent,
        Action<ItemSellCancelledEvent>? modifier = null)
    {
        var entity = MakeEntity(() => new ItemSellCancelledEvent
        {
            InstanceBoughtEvent = boughtEvent
        });

        modifier?.Invoke(entity);

        context.ItemSellCancelledEvents.Add(entity);

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

        orderedEvent.Instance!.InstanceSoldEvents.Add(entity);

        context.ItemSoldEvents.Add(entity);

        return entity;
    }

    public static ItemSellCancelledEvent MockSellCancelledEvent(
        AppDbContext context,
        ItemSoldEvent soldEvent,
        Action<ItemSellCancelledEvent>? modifier = null)
    {
        var entity = MakeEntity(() => new ItemSellCancelledEvent
        {
            InstanceSoldEvent = soldEvent
        });

        modifier?.Invoke(entity);

        context.ItemSellCancelledEvents.Add(entity);

        return entity;
    }
}
