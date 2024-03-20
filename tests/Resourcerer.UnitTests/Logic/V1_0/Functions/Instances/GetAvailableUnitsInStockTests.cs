using Resourcerer.DataAccess.Entities;
using Resourcerer.UnitTests.Utilities.Faker;

namespace Resourcerer.UnitTests.Logic.V1_0.Functions.Instances;

public class GetAvailableUnitsInStockTests : TestsBase
{
    private readonly Func<Instance, double> _sut;
    private readonly Guid _staticId;
    public GetAvailableUnitsInStockTests()
    {
        _sut = Resourcerer.Logic.V1_0.Functions.Instances.GetAvailableUnitsInStock;
        _staticId = Guid.NewGuid();
    }

    //[Fact]
    //public void When_SourceInstance_Delivered_And_DerivedInstance_ReservedTwice_Than_ComputedValue()
    //{
    //    // arrange
    //    var srcInstance = DF.FakeInstance(_ctx, x =>
    //    {
    //        x.Quantity = 2;
    //        x.SourceInstanceId = null;
    //        x.OrderedEvents.Add(DF.FakeInstanceOrderedEvent(_ctx, ev =>
    //        {
    //            ev.DerivedInstanceId = _staticId;
    //            ev.Quantity = 2;
    //            ev.DeliveredEvent = DF.FakeDeliveredEvent();
    //        }));
    //    });

    //    var dervInstance = DF.FakeInstance(_ctx, x =>
    //    {
    //        x.Id = _staticId;
    //        x.Quantity = srcInstance.OrderedEvents[0].Quantity;
    //        x.SourceInstanceId = srcInstance.Id;
    //        x.SourceInstance = srcInstance;
    //        x.ReservedEvents.AddRange(new List<InstanceReservedEvent>
    //        {
    //            DF.FakeReservedEvent(ev => ev.Quantity = 1),
    //            DF.FakeReservedEvent(ev => ev.Quantity = 1)
    //        });
    //    });

    //    // act
    //    var actual = _sut(dervInstance);

    //    // assert
    //    Assert.Equal(0, actual);
    //}

    //[Fact]
    //public void When_SourceInstance_Delivered_And_DerivedInstance_ReservedTwice_OneCancelled_Than_ComputedValue()
    //{
    //    // arrange
    //    var srcInstance = DF.FakeInstance(_ctx, x =>
    //    {
    //        x.Quantity = 2;
    //        x.SourceInstanceId = null;
    //        x.OrderedEvents.Add(DF.FakeInstanceOrderedEvent(_ctx, ev =>
    //        {
    //            ev.DerivedInstanceId = _staticId;
    //            ev.Quantity = 2;
    //            ev.DeliveredEvent = DF.FakeDeliveredEvent();
    //        }));
    //    });

    //    var dervInstance = DF.FakeInstance(_ctx, x =>
    //    {
    //        x.Id = _staticId;
    //        x.Quantity = srcInstance.OrderedEvents[0].Quantity;
    //        x.SourceInstanceId = srcInstance.Id;
    //        x.SourceInstance = srcInstance;
    //        x.ReservedEvents.AddRange(new List<InstanceReservedEvent>
    //        {
    //            DF.FakeReservedEvent(ev => ev.Quantity = 1),
    //            DF.FakeReservedEvent(ev =>
    //            {
    //                ev.Quantity = 1;
    //                ev.CancelledEvent = new InstanceReserveCancelledEvent();
    //            })
    //        });
    //    });

    //    // act
    //    var actual = _sut(dervInstance);

    //    // assert
    //    Assert.Equal(1, actual);
    //}
}
