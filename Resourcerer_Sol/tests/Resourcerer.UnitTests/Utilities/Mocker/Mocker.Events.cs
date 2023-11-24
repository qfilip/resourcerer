using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;

namespace Resourcerer.UnitTests.Utilities.Mocker;

internal static partial class Mocker
{
    public static InstanceBoughtEvent MockOrderedEvent(
        AppDbContext context,
        Action<InstanceBoughtEvent>? modifier = null,
        Item? instanceItem = null)
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

        if(instanceItem != null)
        {
            entity.Instance!.Item = instanceItem;
        }

        context.InstanceBoughtEvents.Add(entity);
        
        return entity;
    }

    public static InstanceCancelledEvent MockOrderCancelledEvent(
        AppDbContext context,
        Action<InstanceCancelledEvent>? modifier = null,
        Item? instanceItem = null)
    {
        var entity = MakeEntity(() => new InstanceCancelledEvent
        {
            InstanceBuyRequestedEvent = MockOrderedEvent(context)
        });

        modifier?.Invoke(entity);

        if (instanceItem != null)
        {
            entity.InstanceBuyRequestedEvent!.Instance!.Item = instanceItem;
        }

        context.InstanceCancelledEvents.Add(entity);

        return entity;
    }

    public static InstanceDeliveredEvent MockDeliveredEvent(
        AppDbContext context,
        Action<InstanceDeliveredEvent>? modifier = null,
        Item? instanceItem = null)
    {
        var entity = MakeEntity(() => new InstanceDeliveredEvent
        {
            InstanceBuyRequestedEvent = MockOrderedEvent(context)
        });

        modifier?.Invoke(entity);

        if (instanceItem != null)
        {
            entity.InstanceBuyRequestedEvent!.Instance!.Item = instanceItem;
        }

        context.InstanceDeliveredEvents.Add(entity);

        return entity;
    }


    public static InstanceSoldEvent MockSellEvent(
        AppDbContext context,
        Action<InstanceSoldEvent>? modifier = null,
        Item? instanceItem = null)
    {
        var entity = MakeEntity(() => new InstanceSoldEvent
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

        if (instanceItem != null)
        {
            entity.Instance!.Item = instanceItem;
        }

        context.Ins.Add(entity);

        return entity;
    }
}
