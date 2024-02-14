using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos;
using Resourcerer.Logic;
using Resourcerer.Logic.Commands.V1_0;
using Resourcerer.UnitTests.Utilities;
using Resourcerer.UnitTests.Utilities.Mocker;

namespace Resourcerer.UnitTests.Logic.V1_0;

public class CreateItemOrderCancelledEventTests : TestsBase
{
    private readonly CreateInstanceOrderCancelledEvent.Handler _handler;
    public CreateItemOrderCancelledEventTests()
    {
        _handler = new(_testDbContext);
    }

    [Fact]
    public void When_AllOk_Then_Ok()
    {
        // arrange
        var sourceInstance = Mocker.MockOrderedEvent(_testDbContext, new Instance());
        _testDbContext.SaveChanges();
        
        var dto = new InstanceCancelRequestDto
        {
            OrderEventId = sourceInstance.OrderedEvents[0].Id,
            InstanceId = sourceInstance.Id,
            Reason = "test"
        };

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Ok, result.Status);
    }

    [Fact]
    public void When_OrderEvent_NotFound_Then_ValidationError()
    {
        var dto = new InstanceCancelRequestDto
        {
            OrderEventId = Guid.NewGuid()
        };

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    [Fact]
    public void When_DeliveredEvent_Exists_Then_ValidationError()
    {
        var sourceInstance = Mocker.MockOrderedEvent(_testDbContext, new Instance());
        var orderEvent = sourceInstance.OrderedEvents[0];
        var _ = Mocker.MockDeliveredEvent(orderEvent);
        _testDbContext.SaveChanges();
        
        var dto = new InstanceCancelRequestDto
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
    public void Is_Idempotent()
    {
        var sourceInstance = Mocker.MockOrderedEvent(_testDbContext, new Instance());
        var orderEvent = sourceInstance.OrderedEvents[0];
        var _ = Mocker.MockDeliveredEvent(orderEvent);
        _testDbContext.SaveChanges();

        var dto = new InstanceCancelRequestDto
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
