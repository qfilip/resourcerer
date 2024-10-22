using Microsoft.EntityFrameworkCore;
using Resourcerer.Application.Models;
using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Entities.JsonEntities;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.V1.Instances.Events.Order;
using Resourcerer.UnitTests.Utilities;

namespace Resourcerer.UnitTests.Logic.V1.Instances.Events.Order;

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
        var sourceInstance = _forger.Fake<Instance>();
        var orderEvent = _forger.Fake<InstanceOrderedEvent>(x =>
        {
            x.Instance = sourceInstance;
            x.SentEvent = AppDbJsonField.CreateKeyless(() => new InstanceOrderSentEvent());
        });

        _ctx.SaveChanges();

        var dto = new V1InstanceOrderDeliverCommand
        {
            InstanceId = sourceInstance.Id,
            OrderEventId = orderEvent.Id
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
        var dto = new V1InstanceOrderDeliverCommand
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
        var sourceInstance = _forger.Fake<Instance>();
        var orderEvent = _forger.Fake<InstanceOrderedEvent>(x =>
        {
            x.Instance = sourceInstance;
            x.CancelledEvent = AppDbJsonField.CreateKeyless(() => new InstanceOrderCancelledEvent());
        });
        var dto = new V1InstanceOrderDeliverCommand
        {
            InstanceId = sourceInstance.Id,
            OrderEventId = orderEvent.Id
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
        var sourceInstance = _forger.Fake<Instance>();
        var orderEvent = _forger.Fake<InstanceOrderedEvent>(x =>
        {
            x.Instance = sourceInstance;
            x.DeliveredEvent = AppDbJsonField.CreateKeyless(() => new InstanceOrderDeliveredEvent());
        });
        _ctx.SaveChanges();

        var dto = new V1InstanceOrderDeliverCommand
        {
            InstanceId = sourceInstance.Id,
            OrderEventId = orderEvent.Id
        };

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    [Fact]
    public void Idempotent_DeliveredEventExist__Ok()
    {
        var sourceInstance = _forger.Fake<Instance>();
        var orderEvent = _forger.Fake<InstanceOrderedEvent>(x =>
        {
            x.Instance = sourceInstance;
            x.SentEvent = AppDbJsonField.CreateKeyless(() => new InstanceOrderSentEvent());
            x.DeliveredEvent = AppDbJsonField.CreateKeyless(() => new InstanceOrderDeliveredEvent());
        });
        _ctx.SaveChanges();

        var dto = new V1InstanceOrderDeliverCommand
        {
            InstanceId = sourceInstance.Id,
            OrderEventId = orderEvent.Id
        };

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Ok, result.Status);
    }
}
