using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;

namespace Resourcerer.UnitTests.Utilities.Mocker;

internal static partial class Mocker
{
    public static ItemOrderedEvent MockBoughtEvent(
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

        context.InstanceBoughtEvents.Add(entity);
        
        return entity;
    }

    public static ItemSellCancelledEvent MockBoughtCancelledEvent(
        AppDbContext context,
        ItemOrderedEvent boughtEvent,
        Action<ItemSellCancelledEvent>? modifier = null)
    {
        var entity = MakeEntity(() => new ItemSellCancelledEvent
        {
            InstanceBoughtEvent = boughtEvent
        });

        modifier?.Invoke(entity);

        context.InstanceCancelledEvents.Add(entity);

        return entity;
    }

    public static ItemDeliveredEvent MockDeliveredEvent(
        AppDbContext context,
        ItemOrderedEvent boughtEvent,
        Action<ItemDeliveredEvent>? modifier = null)
    {
        var entity = MakeEntity(() => new ItemDeliveredEvent
        {
            ItemOrderedEvent = boughtEvent
        });

        modifier?.Invoke(entity);

        context.InstanceDeliveredEvents.Add(entity);

        return entity;
    }

    public static ItemSoldEvent MockSoldEvent(
        AppDbContext context,
        ItemOrderedEvent boughtEvent,
        Action<ItemSoldEvent>? modifier = null)
    {
        var entity = MakeEntity(() => new ItemSoldEvent
        {
            TotalDiscountPercent = 0,
            UnitPrice = 1,
            Quantity = 1,
        });

        modifier?.Invoke(entity);

        boughtEvent.Instance!.InstanceSoldEvents.Add(entity);

        context.InstanceSoldEvents.Add(entity);

        return entity;
    }

    public static ItemSellCancelledEvent MockSoldCancelledEvent(
        AppDbContext context,
        ItemSoldEvent soldEvent,
        Action<ItemSellCancelledEvent>? modifier = null)
    {
        var entity = MakeEntity(() => new ItemSellCancelledEvent
        {
            InstanceSoldEvent = soldEvent
        });

        modifier?.Invoke(entity);

        context.InstanceCancelledEvents.Add(entity);

        return entity;
    }
}
