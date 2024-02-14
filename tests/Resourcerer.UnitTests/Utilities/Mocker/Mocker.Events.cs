using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Logic.Functions.V1_0;

namespace Resourcerer.UnitTests.Utilities.Mocker;

internal static partial class Mocker
{
    public static InstanceOrderedEvent MockOrderedEvent(
        AppDbContext context,
        Action<InstanceOrderedEvent>? modifier = null)
    {
        var sourceInstance = MockInstance(context);
        var derivedInstance = MockInstance(context);
        
        var entity = MakeEntity(() => new InstanceOrderedEvent
        {
            ExpectedDeliveryDate = DateTime.UtcNow,
            TotalDiscountPercent = 0,
            UnitPrice = 1,
            Quantity = 1,

            BuyerCompanyId = derivedInstance.OwnerCompany!.Id,
            SellerCompanyId = sourceInstance.OwnerCompanyId,
            DerivedInstanceId = derivedInstance.Id
        });

        modifier?.Invoke(entity);
        sourceInstance.OrderedEvents.Add(entity);
        derivedInstance.Quantity = entity.Quantity;
        
        return entity;
    }

    public static InstanceOrderCancelledEvent MockOrderCancelledEvent(
        InstanceOrderedEvent orderEv,
        Action<InstanceOrderCancelledEvent>? modifier = null)
    {
        var cancelEv = MakeEntity(() => new InstanceOrderCancelledEvent
        {
            Reason = "test",
            RefundedAmount = 0
        });

        modifier?.Invoke(cancelEv);
        orderEv.OrderCancelledEvent = cancelEv;

        return cancelEv;
    }

    public static InstanceDeliveredEvent MockDeliveredEvent(
        InstanceOrderedEvent orderEv,
        Action<InstanceDeliveredEvent>? modifier = null)
    {
        var deliverEv = MakeEntity(() => new InstanceDeliveredEvent());

        modifier?.Invoke(deliverEv);
        orderEv.DeliveredEvent = deliverEv;

        return deliverEv;
    }

    public static InstanceDiscardedEvent MockDiscardedEvent(
        Instance instance,
        Action<InstanceDiscardedEvent>? modifier = null)
    {
        var discardEv = MakeEntity(() => new InstanceDiscardedEvent()
        {
            Quantity = instance.Quantity,
            Reason = "test"
        });

        modifier?.Invoke(discardEv);
        instance.DiscardedEvents.Add(discardEv);

        return discardEv;
    }
}
