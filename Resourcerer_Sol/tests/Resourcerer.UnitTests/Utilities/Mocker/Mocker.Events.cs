using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;

namespace Resourcerer.UnitTests.Utilities.Mocker;

internal static partial class Mocker
{
    public static InstanceOrderedEvent MockOrderedEvent(AppDbContext context, Action<InstanceOrderedEvent>? modifier = null)
    {
        var entity = MakeEntity(() => new InstanceOrderedEvent
        {
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

        context.InstanceOrderedEvents.Add(entity);
        
        return entity;
    }

    public static InstanceOrderCancelledEvent MockOrderCancelledEvent(AppDbContext context, Action<InstanceOrderCancelledEvent>? modifier = null)
    {
        var entity = MakeEntity(() => new InstanceOrderCancelledEvent
        {
            InstanceOrderedEvent = MockOrderedEvent(context)
        });

        modifier?.Invoke(entity);

        context.InstanceOrderCancelledEvents.Add(entity);

        return entity;
    }

    public static InstanceOrderDeliveredEvent MockDeliveredEvent(AppDbContext context, Action<InstanceOrderDeliveredEvent>? modifier = null)
    {
        var entity = MakeEntity(() => new InstanceOrderDeliveredEvent
        {
            InstanceOrderedEvent = MockOrderedEvent(context)
        });

        modifier?.Invoke(entity);

        context.InstanceOrderDeliveredEvents.Add(entity);

        return entity;
    }
}
