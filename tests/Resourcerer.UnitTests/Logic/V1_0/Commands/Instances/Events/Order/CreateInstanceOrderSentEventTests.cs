using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Entities.JsonEntities;
using Resourcerer.Dtos.Instances.Events.Order;
using Resourcerer.Logic;
using Resourcerer.Logic.V1_0.Commands;
using Resourcerer.UnitTests.Utilities;
using Resourcerer.UnitTests.Utilities.Faker;

namespace Resourcerer.UnitTests.Logic.V1_0.Commands.Instances;

public class CreateInstanceOrderSentEventTests : TestsBase
{
    private readonly CreateInstanceOrderSentEvent.Handler _handler;
    public CreateInstanceOrderSentEventTests()
    {
        _handler = new CreateInstanceOrderSentEvent.Handler(_testDbContext);
    }

    [Fact]
    public void When_AllOk_Then_Ok()
    {
        // arrange
        var sourceInstance = DF.FakeOrderedEvent(_testDbContext, new Instance());
        _testDbContext.SaveChanges();

        var dto = new InstanceOrderSentRequestDto
        {
            OrderEventId = sourceInstance.OrderedEvents[0].Id,
            InstanceId = sourceInstance.Id
        };

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        var instance = _testDbContext.Instances.First(x => x.Id == sourceInstance.Id);
        Assert.Equal(eHandlerResultStatus.Ok, result.Status);
        Assert.NotNull(instance.OrderedEvents[0].SentEvent);
    }

    [Fact]
    public void When_Instance_NotFound_Then_Rejected()
    {
        var sourceInstance = DF.FakeOrderedEvent(_testDbContext, new Instance());
        _testDbContext.SaveChanges();

        var dto = new InstanceOrderSentRequestDto
        {
            OrderEventId = sourceInstance.OrderedEvents[0].Id,
            InstanceId = Guid.NewGuid()
        };

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    [Fact]
    public void When_InstanceOrderEvent_NotFound_Then_Rejected()
    {
        var sourceInstance = DF.FakeOrderedEvent(_testDbContext, new Instance());
        _testDbContext.SaveChanges();

        var dto = new InstanceOrderSentRequestDto
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
    public void When_OrderCancelled_Then_Rejected()
    {
        // arrange
        var sourceInstance = DF.FakeOrderedEvent(_testDbContext, new Instance(), x =>
        {
            x.OrderCancelledEvent = JsonEntityBase.CreateEntity(() => new InstanceOrderCancelledEvent());
        });
        _testDbContext.SaveChanges();

        var dto = new InstanceOrderSentRequestDto
        {
            OrderEventId = sourceInstance.OrderedEvents[0].Id,
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
        var sourceInstance = DF.FakeOrderedEvent(_testDbContext, new Instance(), x =>
        {
            x.SentEvent = JsonEntityBase.CreateEntity(() => new InstanceOrderSentEvent());
        });
        _testDbContext.SaveChanges();

        var dto = new InstanceOrderSentRequestDto
        {
            OrderEventId = sourceInstance.OrderedEvents[0].Id,
            InstanceId = sourceInstance.Id
        };

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Ok, result.Status);
    }

    [Fact]
    public void When_OrderDelivered_Then_Rejected()
    {
        // arrange
        var sourceInstance = DF.FakeOrderedEvent(_testDbContext, new Instance(), x =>
        {
            x.DeliveredEvent = JsonEntityBase.CreateEntity(() => new InstanceOrderDeliveredEvent());
        });
        _testDbContext.SaveChanges();

        var dto = new InstanceOrderSentRequestDto
        {
            OrderEventId = sourceInstance.OrderedEvents[0].Id,
            InstanceId = sourceInstance.Id
        };

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }
}
