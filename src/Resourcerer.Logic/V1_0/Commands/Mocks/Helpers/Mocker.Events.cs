using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;

namespace Resourcerer.Logic.Commands.Mocks.Helpers;

public static partial class Mocker
{
    public static ItemOrderedEvent MockOrderedEvent(
        AppDbContext context,
        Action<ItemOrderedEvent>? modifier = null,
        Item? instanceItem = null)
    {
        var entity = MakeEntity(() => new ItemOrderedEvent
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

    public static ItemCancelledEvent MockOrderCancelledEvent(
        AppDbContext context,
        Action<ItemCancelledEvent>? modifier = null,
        Item? instanceItem = null)
    {
        var entity = MakeEntity(() => new ItemCancelledEvent
        {
            InstanceBoughtEvent = MockOrderedEvent(context)
        });

        modifier?.Invoke(entity);

        if (instanceItem != null)
        {
            entity.InstanceBoughtEvent!.Instance!.Item = instanceItem;
        }

        context.ItemCancelledEvents.Add(entity);

        return entity;
    }

    public static ItemDeliveredEvent MockDeliveredEvent(
        AppDbContext context,
        Action<ItemDeliveredEvent>? modifier = null,
        Item? instanceItem = null)
    {
        var entity = MakeEntity(() => new ItemDeliveredEvent
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
