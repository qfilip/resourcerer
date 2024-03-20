using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic;
using Resourcerer.Logic.V1.Commands;
using Resourcerer.UnitTests.Utilities;
using Resourcerer.UnitTests.Utilities.Faker;

namespace Resourcerer.UnitTests.Logic.V1.Commands.Instances;

public class CreateInstanceOrderDeliveredEventTests : TestsBase
{
    public readonly CreateInstanceOrderDeliveredEvent.Handler _handler;
    public CreateInstanceOrderDeliveredEventTests()
    {
        _handler = new(_ctx);
    }

    [Fact]
    public void HappyPath__Ok()
    {
        // arrange
        var sourceInstance = DF.FakeInstanceOrderedEvent(_ctx, new Instance(), x =>
        {
            x.SentEvent = DF.FakeSentEvent();
        });
        _ctx.SaveChanges();

        var dto = new V1InstanceOrderDeliveredRequest
        {
            InstanceId = sourceInstance.Id,
            OrderEventId = sourceInstance.OrderedEvents.First().Id
        };

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Multiple(
            () => Assert.Equal(eHandlerResultStatus.Ok, result.Status),
            () =>
            {
                _ctx.Clear();
                var instance = _ctx.Instances
                    .Include(x => x.OrderedEvents)
                    .First(x => x.Id == sourceInstance.Id);
                Assert.NotNull(instance.OrderedEvents.First().DeliveredEvent);
            }
        );
    }

    [Fact]
    public void OrderEvent_NotFound__Rejected()
    {
        var dto = new V1InstanceOrderDeliveredRequest
        {
            InstanceId = Guid.NewGuid()
        };

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    [Fact]
    public void CancelledEvent_Exists__Rejected()
    {
        var orderedEvent = DF.FakeInstanceOrderedEvent(_ctx, x => x.CancelledEvent = DF.FakeOrderCancelledEvent());

        var dto = new V1InstanceOrderDeliveredRequest
        {
            InstanceId = orderedEvent.DerivedInstanceId,
            OrderEventId = orderedEvent.Id
        };
        _ctx.SaveChanges();

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    [Fact]
    public void SentEvent_NotExists__Rejected()
    {
        var sourceInstance = DF.FakeInstanceOrderedEvent(_ctx, new Instance(), x =>
        {
            x.DeliveredEvent = DF.FakeDeliveredEvent();
        });
        _ctx.SaveChanges();

        var dto = new V1InstanceOrderDeliveredRequest
        {
            InstanceId = sourceInstance.Id,
            OrderEventId = sourceInstance.OrderedEvents.First().Id
        };

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    [Fact]
    public void Is_Idempotent()
    {
        var sourceInstance = DF.FakeInstanceOrderedEvent(_ctx, new Instance(), x =>
        {
            x.DeliveredEvent = DF.FakeDeliveredEvent();
            x.SentEvent = DF.FakeSentEvent();
        });
        _ctx.SaveChanges();

        var dto = new V1InstanceOrderDeliveredRequest
        {
            InstanceId = sourceInstance.Id,
            OrderEventId = sourceInstance.OrderedEvents.First().Id
        };

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Ok, result.Status);
    }
}
