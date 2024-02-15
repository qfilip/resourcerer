using Resourcerer.Dtos;
using Resourcerer.Logic;
using Resourcerer.Logic.Commands.V1_0;
using Resourcerer.UnitTests.Utilities;
using Resourcerer.UnitTests.Utilities.Faker;

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
        var orderEvent = DF.FakeOrderedEvent(_testDbContext);
        var dto = new InstanceDeliveredRequestDto
        {
            InstanceId = orderEvent.DerivedInstanceId,
            OrderEventId = orderEvent.Id
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
        var orderedEvent = DF.FakeOrderedEvent(_testDbContext, x => x.OrderCancelledEvent = DF.FakeOrderCancelledEvent());
        
        var dto = new InstanceDeliveredRequestDto
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
    public void Is_Idempotent()
    {
        var orderedEvent = DF.FakeOrderedEvent(_testDbContext, x =>
        {
            x.DeliveredEvent = DF.FakeDeliveredEvent(); 
        });
        var dto = new InstanceDeliveredRequestDto
        {
            InstanceId = orderedEvent.DerivedInstanceId,
            OrderEventId = orderedEvent.Id
        };
        _testDbContext.SaveChanges();

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Ok, result.Status);
    }
}
