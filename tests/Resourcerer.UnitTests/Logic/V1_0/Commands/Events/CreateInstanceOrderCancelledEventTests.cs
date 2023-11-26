using Resourcerer.Dtos;
using Resourcerer.Logic;
using Resourcerer.Logic.Commands.V1_0;
using Resourcerer.UnitTests.Utilities;
using Resourcerer.UnitTests.Utilities.Mocker;

namespace Resourcerer.UnitTests.Logic.V1_0;

public class CreateInstanceOrderCancelledEventTests : TestsBase
{
    private readonly CreateItemOrderCancelledEvent.Handler _handler;
    public CreateInstanceOrderCancelledEventTests()
    {
        _handler = new(_testDbContext);
    }

    [Fact]
    public void When_AllOk_Then_Ok()
    {
        // arrange
        var orderEventId = Mocker.MockOrderedEvent(_testDbContext, _sand).Id;
        var dto = new ItemCancelledEventDto
        {
            TargetEventId = orderEventId
        };
        _testDbContext.SaveChanges();

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Ok, result.Status);
    }

    [Fact]
    public void When_OrderEvent_NotFound_Then_ValidationError()
    {
        var dto = new ItemCancelledEventDto
        {
            TargetEventId = Guid.NewGuid()
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
        var dto = new ItemCancelledEventDto
        {
            TargetEventId = deliveredEvent.ItemOrderedEvent!.Id
        };
        _testDbContext.SaveChanges();

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
        var dto = new ItemCancelledEventDto
        {
            TargetEventId = (Guid)orderCancelledEvent.InstanceBoughtEventId!
        };
        _testDbContext.SaveChanges();

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Ok, result.Status);
    }
}
