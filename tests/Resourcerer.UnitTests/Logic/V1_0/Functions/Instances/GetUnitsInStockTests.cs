using Resourcerer.DataAccess.Entities;
using Resourcerer.UnitTests.Utilities.Faker;

namespace Resourcerer.UnitTests.Logic.V1.Functions.Instances;

public class GetUnitsInStockTests : TestsBase
{
    private readonly Func<Instance, double> _sut;
    private readonly Guid _staticId;
    public GetUnitsInStockTests()
    {
        _sut = Resourcerer.Logic.V1_0.Functions.Instances.GetUnitsInStock;
        _staticId = Guid.NewGuid();
    }

    [Fact]
    public void When_SourceInstanceId_IsNull_Then_Computed()
    {
        // arrange
        var instance = DF.FakeInstance(_ctx, x =>
        {
            x.Quantity = 2;
            x.SourceInstanceId = null;
            x.OrderedEvents.Add(DF.FakeInstanceOrderedEvent(_ctx, ev =>
            {
                ev.Quantity = 1;
                ev.SentEvent = DF.FakeSentEvent();
            }));
        });

        // act
        var actual = _sut(instance);
        
        // assert
        Assert.Equal(2, actual);
    }

    [Fact]
    public void When_SourceInstanceId_IsNotNull_And_SourceInstance_IsNull_Then_Exception()
    {
        // arrange
        var instance = DF.FakeInstance(_ctx, x =>
        {
            x.Quantity = 2;
            x.SourceInstanceId = _staticId;
            x.SourceInstance = null;
            x.OrderedEvents.Add(DF.FakeInstanceOrderedEvent(_ctx, ev =>
            {
                ev.Quantity = 1;
                ev.SentEvent = DF.FakeSentEvent();
            }));
        });

        // assert
        Assert.Throws<InvalidOperationException>(() => _sut(instance));
    }

    [Fact]
    public void When_SourceInstance_NotDelivered_Than_Zero()
    {
        // arrange
        var srcInstance = DF.FakeInstance(_ctx, x =>
        {
            x.Quantity = 2;
            x.SourceInstanceId = null;
            x.OrderedEvents.Add(DF.FakeInstanceOrderedEvent(_ctx, ev =>
            {
                ev.DerivedInstanceId = _staticId;
                ev.Quantity = 1;
                ev.SentEvent = DF.FakeSentEvent();
            }));
        });

        var dervInstance = DF.FakeInstance(_ctx, x =>
        {
            x.Id = _staticId;
            x.Quantity = srcInstance.OrderedEvents.First().Quantity;
            x.SourceInstanceId = srcInstance.Id;
            x.SourceInstance = srcInstance;
        });

        // act
        var actual = _sut(dervInstance);

        // assert
        Assert.Equal(0, actual);
    }

    [Fact]
    public void When_SourceInstance_Delivered_Than_ComputedValue()
    {
        // arrange
        var srcInstance = DF.FakeInstance(_ctx, x =>
        {
            x.Quantity = 2;
            x.SourceInstanceId = null;
            x.OrderedEvents.Add(DF.FakeInstanceOrderedEvent(_ctx, ev =>
            {
                ev.DerivedInstanceId = _staticId;
                ev.Quantity = 1;
                ev.DeliveredEvent = DF.FakeDeliveredEvent();
            }));
        });

        var dervInstance = DF.FakeInstance(_ctx, x =>
        {
            x.Id = _staticId;
            x.Quantity = srcInstance.OrderedEvents.First().Quantity;
            x.SourceInstanceId = srcInstance.Id;
            x.SourceInstance = srcInstance;
        });

        // act
        var actual = _sut(dervInstance);

        // assert
        Assert.Equal(1, actual);
    }

    [Fact]
    public void When_SourceInstance_Delivered_And_DerivedInstanceOrdered_Than_ComputedValue()
    {
        // arrange
        var srcInstance = DF.FakeInstance(_ctx, x =>
        {
            x.Quantity = 2;
            x.SourceInstanceId = null;
            x.OrderedEvents.Add(DF.FakeInstanceOrderedEvent(_ctx, ev =>
            {
                ev.DerivedInstanceId = _staticId;
                ev.Quantity = 2;
                ev.DeliveredEvent = DF.FakeDeliveredEvent();
            }));
        });

        var dervInstance = DF.FakeInstance(_ctx, x =>
        {
            x.Id = _staticId;
            x.Quantity = srcInstance.OrderedEvents.First().Quantity;
            x.SourceInstanceId = srcInstance.Id;
            x.SourceInstance = srcInstance;
            x.OrderedEvents.Add(DF.FakeInstanceOrderedEvent(_ctx, ev =>
            {
                ev.Quantity = 1;
            }));
        });

        // act
        var actual = _sut(dervInstance);

        // assert
        Assert.Equal(2, actual);
    }

    [Fact]
    public void When_SourceInstance_Delivered_And_DerivedInstanceOrderedThrice_OnceSent_OnceNot__OnceCancelled_Than_ComputedValue()
    {
        // arrange
        var srcInstance = DF.FakeInstance(_ctx, x =>
        {
            x.Quantity = 3;
            x.SourceInstanceId = null;
            x.OrderedEvents.Add(DF.FakeInstanceOrderedEvent(_ctx, ev =>
            {
                ev.DerivedInstanceId = _staticId;
                ev.Quantity = 3;
                ev.DeliveredEvent = DF.FakeDeliveredEvent();
            }));
        });

        var dervInstance = DF.FakeInstance(_ctx, x =>
        {
            x.Id = _staticId;
            x.Quantity = srcInstance.OrderedEvents.First().Quantity;
            x.SourceInstanceId = srcInstance.Id;
            x.SourceInstance = srcInstance;
            x.OrderedEvents = new List<InstanceOrderedEvent>()
            {
                DF.FakeInstanceOrderedEvent(_ctx, ev =>
                {
                    ev.Quantity = 1;
                }),
                DF.FakeInstanceOrderedEvent(_ctx, ev =>
                {
                    ev.Quantity = 1;
                    ev.CancelledEvent = DF.FakeOrderCancelledEvent();
                }),
                DF.FakeInstanceOrderedEvent(_ctx, ev =>
                {
                    ev.Quantity = 1;
                    ev.SentEvent = DF.FakeSentEvent();
                }),
            };
        });

        // act
        var actual = _sut(dervInstance);

        // assert
        Assert.Equal(2, actual);
    }
}
