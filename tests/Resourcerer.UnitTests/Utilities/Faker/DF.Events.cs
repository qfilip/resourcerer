using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;

namespace Resourcerer.UnitTests.Utilities.Faker;

internal static partial class DF
{
    private static InstanceOrderedEvent CreateInstanceOrderEvent(
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
            DerivedInstanceId = derivedInstance.Id,
            DerivedInstanceItemId = derivedInstance.ItemId,

            Instance = sourceInstance,
            InstanceId = sourceInstance.Id,
        });

        modifier?.Invoke(entity);
        sourceInstance.OrderedEvents.Add(entity);
        derivedInstance.Quantity = entity.Quantity;

        context.Instances.AddRange(new[] { derivedInstance, sourceInstance });

        return entity;
    }
    public static InstanceOrderedEvent FakeInstanceOrderedEvent(
        AppDbContext context,
        Action<InstanceOrderedEvent>? modifier = null)
    {
        var sourceInstance = FakeInstance(context);
        var derivedInstance = FakeInstance(context,
            x => x.SourceInstanceId = sourceInstance.Id);
        
        return CreateInstanceOrderEvent(context, sourceInstance, derivedInstance, modifier);
    }

    public static Instance FakeInstanceOrderedEvent(
        AppDbContext context,
        Instance sourceInstance,
        Action<InstanceOrderedEvent>? modifier = null)
    {
        sourceInstance = FakeInstance(context);
        var derivedInstance = FakeInstance(context,
            x => x.SourceInstanceId = sourceInstance.Id);

        CreateInstanceOrderEvent(context, sourceInstance, derivedInstance, modifier);

        return sourceInstance;
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
        TestDbContext context,
        Action<InstanceReservedEvent>? modifier = null)
    {
        var instance = DF.FakeInstance(context);
        var ev = MakeEntity(() => new InstanceReservedEvent()
        {
            Quantity = 1,
            Reason = "test",

            Instance = instance,
            InstanceId = instance.Id
        });
        modifier?.Invoke(ev);

        context.InstanceReservedEvents.Add(ev);

        return ev;
    }
}
