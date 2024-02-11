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
        var orderEventId = Mocker.MockOrderedEvent(_testDbContext, _sand).Id;
        _testDbContext.SaveChanges();
        
        var dto = new InstanceCancelRequestDto
        {
            OrderEventId = orderEventId
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
        var boughtEvent = Mocker.MockOrderedEvent(_testDbContext, _sand);
        var deliveredEvent = Mocker.MockDeliveredEvent(_testDbContext, boughtEvent);
        _testDbContext.SaveChanges();
        
        var dto = new InstanceCancelRequestDto
        {
            OrderEventId = deliveredEvent.ItemOrderedEvent!.Id
        };

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    [Fact]
    public void Is_Idempotent()
    {
        var boughtEvent = Mocker.MockOrderedEvent(_testDbContext, _sand);
        var orderCancelledEvent = Mocker.MockOrderCancelledEvent(_testDbContext, boughtEvent);
        _testDbContext.SaveChanges();
        
        var dto = new InstanceCancelRequestDto
        {
            OrderEventId = (Guid)orderCancelledEvent.ItemOrderedEventId!
        };

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Ok, result.Status);
    }
}
