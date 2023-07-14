using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Enums;

namespace Resourcerer.UnitTests.Utilities.Mocker;

internal static partial class Mocker
{
    public static InstanceBuyRequestedEvent MockOrderedEvent(
        AppDbContext context,
        eOrderType orderType,
        Action<InstanceBuyRequestedEvent>? modifier = null,
        Item? instanceItem = null)
    {
        var entity = MakeEntity(() => new InstanceBuyRequestedEvent
        {
            OrderType = orderType,
            Instance = MakeEntity(() => new Instance
            {
                ExpiryDate = DateTime.UtcNow,
                ExpectedDeliveryDate = DateTime.UtcNow,
                TotalDiscountPercent = 0,
                UnitPrice = 5,
                UnitsOrdered = 5,
            })
        });

        modifier?.Invoke(entity);

        if(instanceItem != null)
        {
            entity.Instance!.Item = instanceItem;
        }

        context.InstanceOrderedEvents.Add(entity);
        
        return entity;
    }

    public static InstanceRequestCancelledEvent MockOrderCancelledEvent(
        AppDbContext context,
        eOrderType orderType,
        Action<InstanceRequestCancelledEvent>? modifier = null,
        Item? instanceItem = null)
    {
        var entity = MakeEntity(() => new InstanceRequestCancelledEvent
        {
            InstanceBuyRequestedEvent = MockOrderedEvent(context, orderType)
        });

        modifier?.Invoke(entity);

        if (instanceItem != null)
        {
            entity.InstanceBuyRequestedEvent!.Instance!.Item = instanceItem;
        }

        context.InstanceOrderCancelledEvents.Add(entity);

        return entity;
    }

    public static InstanceRequestDeliveredEvent MockDeliveredEvent(
        AppDbContext context,
        eOrderType orderType,
        Action<InstanceRequestDeliveredEvent>? modifier = null,
        Item? instanceItem = null)
    {
        var entity = MakeEntity(() => new InstanceRequestDeliveredEvent
        {
            InstanceBuyRequestedEvent = MockOrderedEvent(context, orderType)
        });

        modifier?.Invoke(entity);

        if (instanceItem != null)
        {
            entity.InstanceBuyRequestedEvent!.Instance!.Item = instanceItem;
        }

        context.InstanceOrderDeliveredEvents.Add(entity);

        return entity;
    }
}
