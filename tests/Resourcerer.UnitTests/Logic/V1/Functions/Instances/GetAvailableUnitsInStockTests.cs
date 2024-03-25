using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Entities.JsonEntities;
using Resourcerer.UnitTests.Utilities.Faker;

namespace Resourcerer.UnitTests.Logic.V1.Functions.Instances;

public class GetAvailableUnitsInStockTests : TestsBase
{
    private readonly Func<Instance, double> _sut;
    public GetAvailableUnitsInStockTests()
    {
        _sut = Resourcerer.Logic.V1.Functions.Instances.GetAvailableUnitsInStock;
    }

    [Fact]
    public void Without_SourceInstance()
    {
        // arrange
        var instance = DF.Fake<Instance>(_ctx, x => x.Quantity = 11);
        var xs = FakeEvents(2, instance);

        _ctx.SaveChanges();

        // act
        var dbInstance = _ctx.Instances
            .Include(x => x.SourceInstance)
            .Include(x => x.OrderedEvents)
            .Include(x => x.ReservedEvents)
            .Include(x => x.DiscardedEvents)
            .First(x => x.Id == instance.Id);
        
        var actual = _sut(dbInstance);

        // assert
        var unavailable = xs.Orders - xs.SentOrders - xs.DeliveredOrders - xs.Reserves - xs.Discards;
        var expected = instance.Quantity - unavailable;
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void With_SourceInstance()
    {
        // arrange
        var sourceInstance = DF.Fake<Instance>(_ctx, x => x.Quantity = 11);
        var instance = DF.Fake<Instance>(_ctx, x =>
        {
            x.Quantity = 0;
            x.SourceInstance = sourceInstance;
        });

        var sourceInstanceEvents = FakeEvents(2, sourceInstance, x => x.DerivedInstanceId = instance.Id);
        var derivedInstanceEvents = FakeEvents(2, instance);

        _ctx.SaveChanges();

        // act
        var dbInstance = _ctx.Instances
            .Include(x => x.SourceInstance)
            .Include(x => x.OrderedEvents)
            .Include(x => x.ReservedEvents)
            .Include(x => x.DiscardedEvents)
            .First(x => x.Id == instance.Id);
        
        var actual = _sut(dbInstance);

        // assert
        var deliveredQty = sourceInstanceEvents.DeliveredOrders;
        var unavailableQty = derivedInstanceEvents.SumAll();

        var expected = deliveredQty - unavailableQty;
        Assert.Equal(expected, actual);
    }

    private AppEvents FakeEvents(int eventCount, Instance instance, Action<InstanceOrderedEvent>? orderModifier = null)
    {
        var orders = FakeEvent<InstanceOrderedEvent>(eventCount, x =>
        {
            x.Instance = instance;
            orderModifier?.Invoke(x);
        });

        var ordersSent = FakeEvent<InstanceOrderedEvent>(eventCount, x =>
        {
            x.SentEvent = AppDbJsonField.Create(() => new InstanceOrderSentEvent());
            x.Instance = instance;
            orderModifier?.Invoke(x);
        });

        var ordersDelivered = FakeEvent<InstanceOrderedEvent>(eventCount, x =>
        {
            x.SentEvent = AppDbJsonField.Create(() => new InstanceOrderSentEvent());
            x.Instance = instance;
            orderModifier?.Invoke(x);
        });

        _ = FakeEvent<InstanceOrderedEvent>(eventCount, x =>
        {
            x.CancelledEvent = AppDbJsonField.Create(() => new InstanceOrderCancelledEvent());
            x.Instance = instance;
        });

        var reservations = FakeEvent<InstanceReservedEvent>(eventCount, x =>
        {
            x.Instance = instance;
        });

        var reservationsUsed = FakeEvent<InstanceReservedEvent>(eventCount, x =>
        {
            x.UsedEvent = AppDbJsonField.Create(() => new InstanceReserveUsedEvent());
            x.Instance = instance;
        });

        _ = FakeEvent<InstanceReservedEvent>(eventCount, x =>
        {
            x.Instance = instance;
            x.CancelledEvent = AppDbJsonField.Create(() => new InstanceReserveCancelledEvent());
        });

        var discards = FakeEvent<InstanceDiscardedEvent>(eventCount, x =>
        {
            x.Instance = instance;
        });


        return new AppEvents(orders, ordersSent, ordersDelivered, reservations, reservationsUsed, discards);
    }

    private TEvent[] FakeEvent<TEvent>(int count, Action<TEvent> modifier) where TEvent : AppDbEntity =>
        Enumerable.Range(0, count)
            .Select(_ => DF.Fake(_ctx, modifier))
            .ToArray();

    private class AppEvents
    {
        public AppEvents(
            InstanceOrderedEvent[] orders,
            InstanceOrderedEvent[] sentOrders,
            InstanceOrderedEvent[] deliveredOrders,
            InstanceReservedEvent[] reserves,
            InstanceReservedEvent[] usedReserves,
            InstanceDiscardedEvent[] discards)
        {
            Orders = orders.Sum(x => x.Quantity);
            SentOrders = sentOrders.Sum(x => x.Quantity);
            DeliveredOrders = deliveredOrders.Sum(x => x.Quantity);
            Reserves = reserves.Sum(x => x.Quantity);
            UsedReserves = usedReserves.Sum(x => x.Quantity);
            Discards = discards.Sum(x => x.Quantity);
        }

        public double Orders { get; private set; }
        public double SentOrders { get; private set; }
        public double DeliveredOrders { get; private set; }
        public double Reserves { get; private set; }
        public double UsedReserves { get; private set; }
        public double Discards { get; private set; }

        public double SumAll() => Orders + SentOrders + DeliveredOrders + Reserves + UsedReserves + Discards;
    }
}
