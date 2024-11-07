using Resourcerer.Logic.Models;
using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Entities.JsonEntities;
using Resourcerer.DataAccess.Utilities.Faking;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.V1.Instances.Events.Order;
using Resourcerer.UnitTests.Utilities;

namespace Resourcerer.UnitTests.Logic.V1.Instances.Events.Order;

public class CreateInstanceOrderSentEventTests : TestsBase
{
    private readonly CreateInstanceOrderSentEvent.Handler _handler;
    public CreateInstanceOrderSentEventTests()
    {
        _handler = new CreateInstanceOrderSentEvent.Handler(_ctx);
    }

    [Fact]
    public void HappyPath__Ok()
    {
        // arrange
        var sourceInstance = _forger.Fake<Instance>();
        var orderEvent = _forger.Fake<InstanceOrderedEvent>(x => x.Instance = sourceInstance);

        _ctx.SaveChanges();

        var dto = new V1InstanceOrderSendCommand
        {
            OrderEventId = orderEvent.Id,
            InstanceId = sourceInstance.Id
        };

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Multiple(
            () => Assert.Equal(eHandlerResultStatus.Ok, result.Status),
            () =>
            {
                var instance = _ctx.Instances.First(x => x.Id == sourceInstance.Id);
                Assert.NotNull(instance.OrderedEvents.First().SentEvent);
            }
        );
    }

    [Fact]
    public void Instance_NotFound__Rejected()
    {
        var sourceInstance = _forger.Fake<Instance>();
        var orderEvent = _forger.Fake<InstanceOrderedEvent>(x => x.Instance = sourceInstance);

        _ctx.SaveChanges();

        var dto = new V1InstanceOrderSendCommand
        {
            OrderEventId = orderEvent.Id,
            InstanceId = Guid.NewGuid()
        };

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    [Fact]
    public void InstanceOrderEvent_NotFound__Rejected()
    {
        var sourceInstance = _forger.Fake<Instance>();
        var orderEvent = _forger.Fake<InstanceOrderedEvent>(x => x.Instance = sourceInstance);

        _ctx.SaveChanges();

        var dto = new V1InstanceOrderSendCommand
        {
            OrderEventId = Guid.NewGuid(),
            InstanceId = sourceInstance.Id
        };

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    [Fact]
    public void OrderCancelled__Rejected()
    {
        // arrange
        var sourceInstance = _forger.Fake<Instance>();
        var orderEvent = _forger.Fake<InstanceOrderedEvent>(x =>
        {
            x.Instance = sourceInstance;
            x.CancelledEvent = AppDbJsonField.CreateKeyless(() => new InstanceOrderCancelledEvent());
        });

        _ctx.SaveChanges();

        var dto = new V1InstanceOrderSendCommand
        {
            OrderEventId = orderEvent.Id,
            InstanceId = sourceInstance.Id
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
        var sourceInstance = _forger.Fake<Instance>();
        var orderEvent = _forger.Fake<InstanceOrderedEvent>(x =>
        {
            x.Instance = sourceInstance;
            x.SentEvent = AppDbJsonField.CreateKeyless(() => new InstanceOrderSentEvent());
        });

        _ctx.SaveChanges();

        var dto = new V1InstanceOrderSendCommand
        {
            OrderEventId = orderEvent.Id,
            InstanceId = sourceInstance.Id
        };

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Ok, result.Status);
    }

    [Fact]
    public void OrderDelivered__Rejected()
    {
        // arrange
        var sourceInstance = _forger.Fake<Instance>();
        var orderEvent = _forger.Fake<InstanceOrderedEvent>(x =>
        {
            x.Instance = sourceInstance;
            x.DeliveredEvent = AppDbJsonField.CreateKeyless(() => new InstanceOrderDeliveredEvent());
        });

        _ctx.SaveChanges();

        var dto = new V1InstanceOrderSendCommand
        {
            OrderEventId = orderEvent.Id,
            InstanceId = sourceInstance.Id
        };

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }
}
