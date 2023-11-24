using Resourcerer.DataAccess.Enums;
using Resourcerer.Dtos;
using Resourcerer.Logic;
using Resourcerer.Logic.Commands.Events;
using Resourcerer.UnitTests.Utilities.Mocker;

namespace Resourcerer.UnitTests.Logic.Commands.Events;

public class CreateInstanceOrderCancelledEventTests : TestsBase
{
    private readonly CreateInstanceOrderCancelledEvent.Handler _handler;
    public CreateInstanceOrderCancelledEventTests()
    {
        _handler = new(_testDbContext);
    }

    [Fact]
    public void When_AllOk_Then_Ok()
    {
        // arrange
        var orderEventId = Mocker.MockBoughtEvent(_testDbContext).Id;
        var dto = new InstanceOrderCancelledEventDto
        {
            InstanceOrderedEventId = orderEventId
        };
        _testDbContext.SaveChanges();

        // act
        var result = _handler.Handle(dto).GetAwaiter().GetResult();

        // assert
        Assert.Equal(eHandlerResultStatus.Ok, result.Status);
    }

    [Fact]
    public void When_OrderEvent_NotFound_Then_ValidationError()
    {
        var dto = new InstanceOrderCancelledEventDto
        {
            InstanceOrderedEventId = Guid.NewGuid()
        };

        // act
        var result = _handler.Handle(dto).GetAwaiter().GetResult();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    [Fact]
    public void When_DeliveredEvent_Exists_Then_ValidationError()
    {
        var deliveredEvent = Mocker.MockDeliveredEvent(_testDbContext);
        var dto = new InstanceOrderCancelledEventDto
        {
            InstanceOrderedEventId = deliveredEvent.InstanceBoughtEvent!.Id
        };
        _testDbContext.SaveChanges();

        // act
        var result = _handler.Handle(dto).GetAwaiter().GetResult();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    [Fact]
    public void Is_Idempotent()
    {
        var orderCancelledEvent = Mocker.MockBoughtCancelledEvent(_testDbContext);
        var dto = new InstanceOrderCancelledEventDto
        {
            InstanceOrderedEventId = (Guid)orderCancelledEvent.InstanceBoughtEventId!
        };
        _testDbContext.SaveChanges();

        // act
        var result = _handler.Handle(dto).GetAwaiter().GetResult();

        // assert
        Assert.Equal(eHandlerResultStatus.Ok, result.Status);
    }
}
