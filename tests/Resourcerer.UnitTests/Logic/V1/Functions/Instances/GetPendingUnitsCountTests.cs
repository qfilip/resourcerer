using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Entities.JsonEntities;

namespace Resourcerer.UnitTests.Logic.V1.Functions.Instances;

public class GetPendingUnitsCountTests : TestsBase
{
    private readonly Item _item;
    private readonly Company _ownerCompany;
    private readonly IQueryable<Instance> _query;
    private readonly Func<IEnumerable<Instance>, double> _sut;

    public GetPendingUnitsCountTests()
    {
        _item = _forger.Fake<Item>();
        _ownerCompany = _forger.Fake<Company>();
        _query = Resourcerer.Logic.V1.Functions.Instances.GetAvailableUnitsInStockDbQuery(_ctx.Instances);
        _sut = Resourcerer.Logic.V1.Functions.Instances.GetPendingUnits;
    }

    [Fact]
    public void HappyPath__Ok()
    {
        // arrange
        _forger.Fake<Instance>(x =>
        {
            x.Quantity = 3;
            x.OrderedEvents = new List<InstanceOrderedEvent>()
            {
                FakeEvent(x),
                FakeEvent(x, ev =>
                {
                    ev.SentEvent = AppDbJsonField.Create<InstanceOrderSentEvent>();
                }),
                FakeEvent(x, ev =>
                {
                    ev.SentEvent = AppDbJsonField.Create<InstanceOrderSentEvent>();
                    ev.DeliveredEvent = AppDbJsonField.Create<InstanceOrderDeliveredEvent>();
                })
            };
        });

        _ctx.SaveChanges();

        // act
        var instances = _query.Where(x => x.ItemId == _item.Id).ToArray();
        var actual = _sut(instances);

        // assert
        Assert.Equal(2, actual);
    }

    private InstanceOrderedEvent FakeEvent(Instance sourceInstance, Action<InstanceOrderedEvent>? modifier = null)
    {
        var @event = _forger.Fake<InstanceOrderedEvent>(ev =>
        {
            ev.Quantity = 1;
            ev.DerivedInstanceId = _forger.Fake<Instance>(i =>
            {
                i.Quantity = ev.Quantity;
                i.Item = _item;
                i.SourceInstance = sourceInstance;
            }).Id;
        });

        modifier?.Invoke(@event);

        return @event;
    }
}
