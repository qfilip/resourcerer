using Resourcerer.Dtos;
using Resourcerer.Logic;
using Resourcerer.Logic.Commands.Events;
using Resourcerer.UnitTests.Utilities.Mocker;

namespace Resourcerer.UnitTests.Logic.Commands.Events;

public class CreateInstanceDeliveredEventTests : TestsBase
{
    public readonly CreateInstanceDeliveredEvent.Handler _handler;
    public CreateInstanceDeliveredEventTests()
    {
        _handler = new(_testDbContext);
    }

    [Fact]
    public void When_AllOk_Then_Ok()
    {
        // arrange
        var orderEventId = Mocker.MockOrderedEvent(_testDbContext).Id;
        var dto = new InstanceDeliveredEventDto
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
        var dto = new InstanceDeliveredEventDto
        {
            InstanceOrderedEventId = Guid.NewGuid()
        };

        // act
        var result = _handler.Handle(dto).GetAwaiter().GetResult();

        // assert
        Assert.Equal(eHandlerResultStatus.ValidationError, result.Status);
    }

    [Fact]
    public void When_CancelledEvent_Exists_Then_ValidationError()
    {
        var orderEventId = Mocker.MockOrderedEvent(_testDbContext).Id;
        Mocker.MockOrderCancelledEvent(_testDbContext, x => x.InstanceOrderedEventId = orderEventId);
        var dto = new InstanceDeliveredEventDto
        {
            InstanceOrderedEventId = orderEventId
        };
        _testDbContext.SaveChanges();

        // act
        var result = _handler.Handle(dto).GetAwaiter().GetResult();

        // assert
        Assert.Equal(eHandlerResultStatus.ValidationError, result.Status);
    }

    [Fact]
    public void Is_Idempotent()
    {
        var orderCancelledEvent = Mocker.MockDeliveredEvent(_testDbContext);
        var dto = new InstanceDeliveredEventDto
        {
            InstanceOrderedEventId = orderCancelledEvent.InstanceOrderedEventId
        };
        _testDbContext.SaveChanges();

        // act
        var result = _handler.Handle(dto).GetAwaiter().GetResult();

        // assert
        Assert.Equal(eHandlerResultStatus.Ok, result.Status);
    }
}
