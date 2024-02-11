using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;

namespace Resourcerer.Logic.Commands.Mocks.Helpers;

public static partial class Mocker
{
    public static InstanceOrderedEvent MockOrderedEvent(
        AppDbContext context,
        Action<InstanceOrderedEvent>? modifier = null,
        Item? instanceItem = null)
    {
        var entity = MakeEntity(() => new InstanceOrderedEvent
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

        context.ItemOrderedEvents.Add(entity);
        
        return entity;
    }

    public static InstanceOrderCancelledEvent MockOrderCancelledEvent(
        AppDbContext context,
        Action<InstanceOrderCancelledEvent>? modifier = null,
        Item? instanceItem = null)
    {
        var entity = MakeEntity(() => new InstanceOrderCancelledEvent
        {
            ItemOrderedEvent = MockOrderedEvent(context)
        });

        modifier?.Invoke(entity);

        if (instanceItem != null)
        {
            entity.ItemOrderedEvent!.Instance!.Item = instanceItem;
        }

        context.ItemCancelledEvents.Add(entity);

        return entity;
    }

    public static InstanceDeliveredEvent MockDeliveredEvent(
        AppDbContext context,
        Action<InstanceDeliveredEvent>? modifier = null,
        Item? instanceItem = null)
    {
        var entity = MakeEntity(() => new InstanceDeliveredEvent
        {
            ItemOrderedEvent = MockOrderedEvent(context)
        });

        modifier?.Invoke(entity);

        if (instanceItem != null)
        {
            entity.ItemOrderedEvent!.Instance!.Item = instanceItem;
        }

        context.ItemDeliveredEvents.Add(entity);

        return entity;
    }
}
