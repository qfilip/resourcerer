using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;

namespace Resourcerer.UnitTests.Utilities.Mocker;

internal static partial class Mocker
{
    public static InstanceBoughtEvent MockBoughtEvent(
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

    public static InstanceCancelledEvent MockBoughtCancelledEvent(
        AppDbContext context,
        Action<InstanceCancelledEvent>? modifier = null,
        Item? instanceItem = null)
    {
        var entity = MakeEntity(() => new InstanceCancelledEvent
        {
            InstanceBoughtEvent = MockBoughtEvent(context)
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
            InstanceBoughtEvent = MockBoughtEvent(context)
        });

        modifier?.Invoke(entity);

        if (instanceItem != null)
        {
            entity.InstanceBoughtEvent!.Instance!.Item = instanceItem;
        }

        context.InstanceDeliveredEvents.Add(entity);

        return entity;
    }

    public static InstanceSoldEvent MockSoldEvent(
        AppDbContext context,
        Action<InstanceSoldEvent>? modifier = null,
        Item? instanceItem = null)
    {
        var entity = MakeEntity(() => new InstanceSoldEvent
        {
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

        context.InstanceSoldEvents.Add(entity);

        return entity;
    }

    public static InstanceCancelledEvent MockSoldCancelledEvent(
        AppDbContext context,
        Action<InstanceCancelledEvent>? modifier = null,
        Item? instanceItem = null)
    {
        var entity = MakeEntity(() => new InstanceCancelledEvent
        {
            InstanceSoldEvent = MockSoldEvent(context)
        });

        modifier?.Invoke(entity);

        if (instanceItem != null)
        {
            entity.InstanceSoldEvent!.Instance!.Item = instanceItem;
        }

        context.InstanceCancelledEvents.Add(entity);

        return entity;
    }
}
