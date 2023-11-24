using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;

namespace Resourcerer.Logic.Commands.Mocks.Helpers;

public static partial class Mocker
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
            InstanceBoughtEvent = MockOrderedEvent(context)
        });

        modifier?.Invoke(entity);

        if (instanceItem != null)
        {
            entity.InstanceBoughtEvent!.Instance!.Item = instanceItem;
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
            InstanceBoughtEvent = MockOrderedEvent(context)
        });

        modifier?.Invoke(entity);

        if (instanceItem != null)
        {
            entity.InstanceBoughtEvent!.Instance!.Item = instanceItem;
        }

        context.InstanceDeliveredEvents.Add(entity);

        return entity;
    }
}
