using Resourcerer.Dtos;
using Resourcerer.Logic;
using Resourcerer.Logic.Commands.V1_0;
using Resourcerer.UnitTests.Utilities;
using Resourcerer.UnitTests.Utilities.Mocker;

namespace Resourcerer.UnitTests.Logic.V1_0;

public class CreateItemDeliveredEventTests : TestsBase
{
    public readonly CreateInstanceDeliveredEvent.Handler _handler;
    public CreateItemDeliveredEventTests()
    {
        _handler = new(_testDbContext);
    }

    [Fact]
    public void When_AllOk_Then_Ok()
    {
        // arrange
        var orderEventId = Mocker.MockOrderedEvent(_testDbContext, _sand).Id;
        var dto = new InstanceDeliveredRequestDto
        {
            InstanceId = orderEventId
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
        var dto = new InstanceDeliveredRequestDto
        {
            InstanceId = Guid.NewGuid()
        };

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    [Fact]
    public void When_CancelledEvent_Exists_Then_ValidationError()
    {
        var boughtEvent = Mocker.MockOrderedEvent(_testDbContext, _sand);
        var cancelledEvent = Mocker.MockOrderCancelledEvent(_testDbContext, boughtEvent);
        var dto = new InstanceDeliveredRequestDto
        {
            InstanceId = cancelledEvent.ItemOrderedEvent!.Id
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
        var orderCancelledEvent = Mocker.MockDeliveredEvent(_testDbContext, boughtEvent);
        var dto = new InstanceDeliveredRequestDto
        {
            InstanceId = (Guid)orderCancelledEvent.ItemOrderedEventId!
        };
        _testDbContext.SaveChanges();

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Ok, result.Status);
    }
}
