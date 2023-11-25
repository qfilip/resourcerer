using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;

namespace Resourcerer.UnitTests.Utilities.Mocker;

internal static partial class Mocker
{
    public static InstanceBoughtEvent MockBoughtEvent(
        AppDbContext context,
        Item item,
        Action<InstanceBoughtEvent>? modifier = null)
    {
        var entity = MakeEntity(() => new InstanceBoughtEvent
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

    public static InstanceCancelledEvent MockBoughtCancelledEvent(
        AppDbContext context,
        InstanceBoughtEvent boughtEvent,
        Action<InstanceCancelledEvent>? modifier = null)
    {
        var entity = MakeEntity(() => new InstanceCancelledEvent
        {
            InstanceBoughtEvent = boughtEvent
        });

        modifier?.Invoke(entity);

        context.InstanceCancelledEvents.Add(entity);

        return entity;
    }

    public static InstanceDeliveredEvent MockDeliveredEvent(
        AppDbContext context,
        InstanceBoughtEvent boughtEvent,
        Action<InstanceDeliveredEvent>? modifier = null)
    {
        var entity = MakeEntity(() => new InstanceDeliveredEvent
        {
            InstanceBoughtEvent = boughtEvent
        });

        modifier?.Invoke(entity);

        context.InstanceDeliveredEvents.Add(entity);

        return entity;
    }

    public static InstanceSoldEvent MockSoldEvent(
        AppDbContext context,
        InstanceBoughtEvent boughtEvent,
        Action<InstanceSoldEvent>? modifier = null)
    {
        var entity = MakeEntity(() => new InstanceSoldEvent
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

    public static InstanceCancelledEvent MockSoldCancelledEvent(
        AppDbContext context,
        InstanceSoldEvent soldEvent,
        Action<InstanceCancelledEvent>? modifier = null)
    {
        var entity = MakeEntity(() => new InstanceCancelledEvent
        {
            InstanceSoldEvent = soldEvent
        });

        modifier?.Invoke(entity);

        context.InstanceCancelledEvents.Add(entity);

        return entity;
    }
}
