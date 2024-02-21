using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.Instances.Events.Order;
using Resourcerer.Logic;
using Resourcerer.Logic.Commands.V1_0;
using Resourcerer.UnitTests.Utilities;
using Resourcerer.UnitTests.Utilities.Faker;

namespace Resourcerer.UnitTests.Logic.V1_0;

public class CreateItemDeliveredEventTests : TestsBase
{
    public readonly CreateInstanceOrderDeliveredEvent.Handler _handler;
    public CreateItemDeliveredEventTests()
    {
        _handler = new(_testDbContext);
    }

    [Fact]
    public void When_AllOk_Then_Ok()
    {
        // arrange
        var sourceInstance = DF.FakeOrderedEvent(_testDbContext, new Instance(), x =>
        {
            x.DeliveredEvent = DF.FakeDeliveredEvent();
            x.SentEvent = DF.FakeSentEvent();
        });
        _testDbContext.SaveChanges();

        var dto = new InstanceOrderDeliveredRequestDto
        {
            InstanceId = sourceInstance.Id,
            OrderEventId = sourceInstance.OrderedEvents[0].Id
        };

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Ok, result.Status);
    }

    [Fact]
    public void When_OrderEvent_NotFound_Then_Rejected()
    {
        var dto = new InstanceOrderDeliveredRequestDto
        {
            InstanceId = Guid.NewGuid()
        };

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    [Fact]
    public void When_CancelledEvent_Exists_Then_Rejected()
    {
        var orderedEvent = DF.FakeOrderedEvent(_testDbContext, x => x.OrderCancelledEvent = DF.FakeOrderCancelledEvent());
        
        var dto = new InstanceOrderDeliveredRequestDto
        {
            InstanceId = orderedEvent.DerivedInstanceId,
            OrderEventId = orderedEvent.Id
        };
        _testDbContext.SaveChanges();

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    [Fact]
    public void When_SentEvent_NotExists_Then_Rejected()
    {
        var sourceInstance = DF.FakeOrderedEvent(_testDbContext, new Instance(), x =>
        {
            x.DeliveredEvent = DF.FakeDeliveredEvent();
        });
        _testDbContext.SaveChanges();

        var dto = new InstanceOrderDeliveredRequestDto
        {
            InstanceId = sourceInstance.Id,
            OrderEventId = sourceInstance.OrderedEvents[0].Id
        };

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    [Fact]
    public void Is_Idempotent()
    {
        var sourceInstance = DF.FakeOrderedEvent(_testDbContext, new Instance(), x =>
        {
            x.DeliveredEvent = DF.FakeDeliveredEvent(); 
            x.SentEvent = DF.FakeSentEvent();
        });
        _testDbContext.SaveChanges();
        
        var dto = new InstanceOrderDeliveredRequestDto
        {
            InstanceId = sourceInstance.Id,
            OrderEventId = sourceInstance.OrderedEvents[0].Id
        };

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Ok, result.Status);
    }
}
