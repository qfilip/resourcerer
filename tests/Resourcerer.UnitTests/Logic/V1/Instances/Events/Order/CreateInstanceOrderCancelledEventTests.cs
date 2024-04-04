using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Entities.JsonEntities;
using Resourcerer.DataAccess.Utilities.Faking;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic;
using Resourcerer.Logic.V1.Instances.Events.Order;
using Resourcerer.UnitTests.Utilities;

namespace Resourcerer.UnitTests.Logic.V1.Instances.Events.Order;

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
        var sourceInstance = DF.Fake<Instance>(_ctx);
        var orderEvent = DF.Fake<InstanceOrderedEvent>(_ctx, x => x.Instance = sourceInstance);
        _ctx.SaveChanges();

        var dto = new V1InstanceOrderCancelRequest
        {
            OrderEventId = orderEvent.Id,
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
    public void OrderEvent_NotFound__NotFound()
    {
        var dto = new V1InstanceOrderCancelRequest
        {
            OrderEventId = Guid.NewGuid()
        };

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.NotFound, result.Status);
    }

    [Fact]
    public void DeliveredEvent_Exists__Rejected()
    {
        var sourceInstance = DF.Fake<Instance>(_ctx);
        var orderEvent = DF.Fake<InstanceOrderedEvent>(_ctx, x =>
        {
            x.Instance = sourceInstance;
            x.DeliveredEvent = AppDbJsonField.Create(() => new InstanceOrderDeliveredEvent());
        });

        _ctx.SaveChanges();

        var dto = new V1InstanceOrderCancelRequest
        {
            OrderEventId = orderEvent.Id,
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
        var sourceInstance = DF.Fake<Instance>(_ctx);
        var orderEvent = DF.Fake<InstanceOrderedEvent>(_ctx, x =>
        {
            x.Instance = sourceInstance;
            x.SentEvent = AppDbJsonField.Create(() => new InstanceOrderSentEvent());
        });

        _ctx.SaveChanges();

        var dto = new V1InstanceOrderCancelRequest
        {
            OrderEventId = orderEvent.Id,
            InstanceId = sourceInstance.Id,
            Reason = "test"
        };

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    [Fact]
    public void Idempotent_CancelEventExists__Ok()
    {
        // arrange
        var sourceInstance = DF.Fake<Instance>(_ctx);
        var orderEvent = DF.Fake<InstanceOrderedEvent>(_ctx, x => x.Instance = sourceInstance);

        _ctx.SaveChanges();

        var dto = new V1InstanceOrderCancelRequest
        {
            OrderEventId = orderEvent.Id,
            InstanceId = sourceInstance.Id,
            Reason = "test"
        };

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Ok, result.Status);
    }
}
