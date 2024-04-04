using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Entities.JsonEntities;
using Resourcerer.DataAccess.Utilities.Faking;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic;
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
        var sourceInstance = DF.Fake<Instance>(_ctx);
        var orderEvent = DF.Fake<InstanceOrderedEvent>(_ctx, x =>
        {
            x.Instance = sourceInstance;
            x.SentEvent = AppDbJsonField.Create(() => new InstanceOrderSentEvent());
        });

        _ctx.SaveChanges();

        var dto = new V1InstanceOrderDeliveredRequest
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
        var sourceInstance = DF.Fake<Instance>(_ctx);
        var orderEvent = DF.Fake<InstanceOrderedEvent>(_ctx, x =>
        {
            x.Instance = sourceInstance;
            x.CancelledEvent = AppDbJsonField.Create(() => new InstanceOrderCancelledEvent());
        });
        var dto = new V1InstanceOrderDeliveredRequest
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
        var sourceInstance = DF.Fake<Instance>(_ctx);
        var orderEvent = DF.Fake<InstanceOrderedEvent>(_ctx, x =>
        {
            x.Instance = sourceInstance;
            x.DeliveredEvent = AppDbJsonField.Create(() => new InstanceOrderDeliveredEvent());
        });
        _ctx.SaveChanges();

        var dto = new V1InstanceOrderDeliveredRequest
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
        var sourceInstance = DF.Fake<Instance>(_ctx);
        var orderEvent = DF.Fake<InstanceOrderedEvent>(_ctx, x =>
        {
            x.Instance = sourceInstance;
            x.SentEvent = AppDbJsonField.Create(() => new InstanceOrderSentEvent());
            x.DeliveredEvent = AppDbJsonField.Create(() => new InstanceOrderDeliveredEvent());
        });
        _ctx.SaveChanges();

        var dto = new V1InstanceOrderDeliveredRequest
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
