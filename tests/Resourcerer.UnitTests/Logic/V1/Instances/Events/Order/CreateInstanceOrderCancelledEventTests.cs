using Microsoft.EntityFrameworkCore;
using Resourcerer.Logic.Models;
using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Entities.JsonEntities;
using Resourcerer.Dtos.V1;
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
        var sourceInstance = _forger.Fake<Instance>();
        var orderEvent = _forger.Fake<InstanceOrderedEvent>(x => x.Instance = sourceInstance);
        _ctx.SaveChanges();

        var dto = new V1InstanceOrderCancelCommand
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
        var dto = new V1InstanceOrderCancelCommand
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
        var sourceInstance = _forger.Fake<Instance>();
        var orderEvent = _forger.Fake<InstanceOrderedEvent>(x =>
        {
            x.Instance = sourceInstance;
            x.DeliveredEvent = AppDbJsonField.CreateKeyless(() => new InstanceOrderDeliveredEvent());
        });

        _ctx.SaveChanges();

        var dto = new V1InstanceOrderCancelCommand
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
        var sourceInstance = _forger.Fake<Instance>();
        var orderEvent = _forger.Fake<InstanceOrderedEvent>(x =>
        {
            x.Instance = sourceInstance;
            x.SentEvent = AppDbJsonField.CreateKeyless(() => new InstanceOrderSentEvent());
        });

        _ctx.SaveChanges();

        var dto = new V1InstanceOrderCancelCommand
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
        var sourceInstance = _forger.Fake<Instance>();
        var orderEvent = _forger.Fake<InstanceOrderedEvent>(x => x.Instance = sourceInstance);

        _ctx.SaveChanges();

        var dto = new V1InstanceOrderCancelCommand
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
