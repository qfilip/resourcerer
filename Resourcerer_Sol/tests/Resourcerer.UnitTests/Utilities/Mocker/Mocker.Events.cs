using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;

namespace Resourcerer.UnitTests.Utilities.Mocker;

internal static partial class Mocker
{
    public static InstanceOrderedEvent MockInstanceOrderedEvent(AppDbContext context, Action<InstanceOrderedEvent>? modifier = null)
    {
        var entity = new InstanceOrderedEvent
        {
            Id = Guid.NewGuid(),

            Instance = new Instance
            {
                Id = Guid.NewGuid(),
                ExpiryDate = DateTime.UtcNow,
                ExpectedDeliveryDate = DateTime.UtcNow,
                TotalDiscountPercent = 0,
                UnitPrice = 5,
                UnitsOrdered = 5,
            }
        };

        if(modifier != null)
            modifier(entity);
        else
            entity.Instance.Item = MockItem(context);

        context.InstanceOrderedEvents.Add(entity);
        
        return entity;
    }

    public static InstanceOrderDeliveredEvent MockInstanceOrderDeliveredEvent(AppDbContext context, Action<InstanceOrderDeliveredEvent>? modifier = null)
    {
        var entity = new InstanceOrderDeliveredEvent
        {
            InstanceOrderedEventId = MockInstanceOrderedEvent(context).Id
        };

        modifier?.Invoke(entity);

        context.InstanceOrderDeliveredEvents.Add(entity);

        return entity;
    }
}
