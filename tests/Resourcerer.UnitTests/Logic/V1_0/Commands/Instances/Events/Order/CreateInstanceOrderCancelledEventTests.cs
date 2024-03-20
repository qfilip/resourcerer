using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic;
using Resourcerer.Logic.V1.Commands;
using Resourcerer.UnitTests.Utilities;
using Resourcerer.UnitTests.Utilities.Faker;
using Resourcerer.Utilities;

namespace Resourcerer.UnitTests.Logic.V1.Commands.Instances;

public class CreateInstanceOrderCancelledEventTests : TestsBase
{
    private readonly CreateInstanceOrderCancelledEvent.Handler _handler;
    public CreateInstanceOrderCancelledEventTests()
    {
        _handler = new(_ctx);
    }

    [Fact]
    public void HappyPath__Ok()
    {
        // arrange
        var sourceInstance = DF.FakeInstanceOrderedEvent(_ctx, new Instance());
        _ctx.SaveChanges();

        var dto = new V1InstanceOrderCancelRequest
        {
            OrderEventId = sourceInstance.OrderedEvents.First().Id,
            InstanceId = sourceInstance.Id,
            Reason = "test"
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
                    .First(x => x.Id == sourceInstance.Id)!;

                Assert.NotNull(instance.OrderedEvents.First().CancelledEvent);
            }
        );
    }

    [Fact]
    public void OrderEvent_NotFound__Rejected()
    {
        var dto = new V1InstanceOrderCancelRequest
        {
            OrderEventId = Guid.NewGuid()
        };

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    [Fact]
    public void DeliveredEvent_Exists__Rejected()
    {
        var sourceInstance = DF.FakeInstanceOrderedEvent(_ctx, new Instance(), x =>
        {
            x.DeliveredEvent = DF.FakeDeliveredEvent();
        });

        _ctx.SaveChanges();

        var dto = new V1InstanceOrderCancelRequest
        {
            OrderEventId = sourceInstance.OrderedEvents.First().Id,
            InstanceId = sourceInstance.Id,
            Reason = "test"
        };

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    [Fact]
    public void SentEvent_Exists__Rejected()
    {
        var sourceInstance = DF.FakeInstanceOrderedEvent(_ctx, new Instance(), x =>
        {
            x.SentEvent = DF.FakeSentEvent();
        });

        _ctx.SaveChanges();

        var dto = new V1InstanceOrderCancelRequest
        {
            OrderEventId = sourceInstance.OrderedEvents.First().Id,
            InstanceId = sourceInstance.Id,
            Reason = "test"
        };

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    [Fact]
    public void Is_Idempotent()
    {
        // arrange
        var sourceInstance = DF.FakeInstanceOrderedEvent(_ctx, new Instance());
        _ctx.SaveChanges();

        var dto = new V1InstanceOrderCancelRequest
        {
            OrderEventId = sourceInstance.OrderedEvents.First().Id,
            InstanceId = sourceInstance.Id,
            Reason = "test"
        };

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Ok, result.Status);
    }
}
