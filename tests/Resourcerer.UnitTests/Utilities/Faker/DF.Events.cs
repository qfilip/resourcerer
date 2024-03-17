using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;

namespace Resourcerer.UnitTests.Utilities.Faker;

internal static partial class DF
{
    private static InstanceOrderedEvent CreateOrder(
        AppDbContext context,
        Instance sourceInstance,
        Instance derivedInstance,
        Action<InstanceOrderedEvent>? modifier = null
    )
    {
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

        context.Instances.AddRange(new[] { derivedInstance, sourceInstance });

        return entity;
    }
    public static InstanceOrderedEvent FakeOrderedEvent(
        AppDbContext context,
        Action<InstanceOrderedEvent>? modifier = null)
    {
        var sourceInstance = FakeInstance(context);
        var derivedInstance = FakeInstance(context,
            x => x.SourceInstanceId = sourceInstance.Id);
        
        return CreateOrder(context, sourceInstance, derivedInstance, modifier);
    }

    public static Instance FakeOrderedEvent(
        AppDbContext context,
        Instance sourceInstance,
        Action<InstanceOrderedEvent>? modifier = null)
    {
        sourceInstance = FakeInstance(context);
        var derivedInstance = FakeInstance(context,
            x => x.SourceInstanceId = sourceInstance.Id);

        CreateOrder(context, sourceInstance, derivedInstance, modifier);

        return sourceInstance;
    }

    public static InstanceOrderCancelledEvent FakeOrderCancelledEvent(
        Action<InstanceOrderCancelledEvent>? modifier = null)
    {
        var cancelEv = MakeEntity(() => new InstanceOrderCancelledEvent
        {
            Reason = "test",
            RefundedAmount = 0
        });

        modifier?.Invoke(cancelEv);

        return cancelEv;
    }

    public static InstanceOrderSentEvent FakeSentEvent(
        Action<InstanceOrderSentEvent>? modifier = null)
    {
        var sentEv = MakeEntity(() => new InstanceOrderSentEvent());
        modifier?.Invoke(sentEv);

        return sentEv;
    }

    public static InstanceOrderDeliveredEvent FakeDeliveredEvent(
        Action<InstanceOrderDeliveredEvent>? modifier = null)
    {
        var deliverEv = MakeEntity(() => new InstanceOrderDeliveredEvent());
        modifier?.Invoke(deliverEv);

        return deliverEv;
    }

    public static InstanceDiscardedEvent FakeDiscardedEvent(
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

    public static InstanceReservedEvent FakeReservedEvent(
        Action<InstanceReservedEvent>? modifier = null)
    {
        var ev = MakeEntity(() => new InstanceReservedEvent());
        modifier?.Invoke(ev);

        return ev;
    }
}
