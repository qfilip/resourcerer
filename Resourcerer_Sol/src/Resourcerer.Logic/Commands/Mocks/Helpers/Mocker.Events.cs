using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;

namespace Resourcerer.Logic.Commands.Mocks.Helpers;

public static partial class Mocker
{
    public static InstanceBuyRequestedEvent MockOrderedEvent(
        AppDbContext context,
        Action<InstanceBuyRequestedEvent>? modifier = null,
        Item? instanceItem = null)
    {
        var entity = MakeEntity(() => new InstanceBuyRequestedEvent
        {
            ExpectedDeliveryDate = DateTime.UtcNow,
            TotalDiscountPercent = 0,
            UnitPrice = 5,
            Quantity = 5,

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

        context.InstanceOrderedEvents.Add(entity);
        
        return entity;
    }

    public static InstanceRequestCancelledEvent MockOrderCancelledEvent(
        AppDbContext context,
        Action<InstanceRequestCancelledEvent>? modifier = null,
        Item? instanceItem = null)
    {
        var entity = MakeEntity(() => new InstanceRequestCancelledEvent
        {
            InstanceBuyRequestedEvent = MockOrderedEvent(context)
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
        Action<InstanceRequestDeliveredEvent>? modifier = null,
        Item? instanceItem = null)
    {
        var entity = MakeEntity(() => new InstanceRequestDeliveredEvent
        {
            InstanceBuyRequestedEvent = MockOrderedEvent(context)
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
