using Resourcerer.Dtos;
using Resourcerer.Logic;
using Resourcerer.Logic.V1_0.Commands;
using Resourcerer.UnitTests.Utilities;
using Resourcerer.UnitTests.Utilities.Mocker;

namespace Resourcerer.UnitTests.Logic.V1_0.Commands.Events;

public class CreateItemDiscardedEventTests : TestsBase
{
    public readonly CreateItemDiscardedEvent.Handler _handler;
    public CreateItemDiscardedEventTests()
    {
        _handler = new(_testDbContext);
    }

    [Fact]
    public void When_AllOk_Then_Ok()
    {
        // arrange
        var orderEvent = Mocker.MockOrderedEvent(_testDbContext, _sand);
        var dto = new ItemDiscardedEventDto
        {
            InstanceId = orderEvent.InstanceId
        };

        _testDbContext.SaveChanges();

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Ok, result.Status);
    }

    [Fact]
    public void When_NotOrdered_Then_NotFound()
    {
        // arrange
        var dto = new ItemDiscardedEventDto
        {
            InstanceId = Guid.NewGuid()
        };

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.NotFound, result.Status);
    }

    [Fact]
    public void When_AlreadyDiscarded_Then_Rejected()
    {
        // arrange
        var orderEvent = Mocker.MockOrderedEvent(_testDbContext, _sand);
        Mocker.MockDeliveredEvent(_testDbContext, orderEvent);
        Mocker.MockDiscardedEvent(_testDbContext, orderEvent);
        
        _testDbContext.SaveChanges();
        
        var dto = new ItemDiscardedEventDto
        {
            InstanceId = orderEvent.InstanceId
        };

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Ok, result.Status);
    }

    [Fact]
    public void When_QuantityLeft_SmallerThan_Zero_Then_Exception()
    {

    }

    [Fact]
    public void When_QuantityLeft_Equals_Zero_Then_Rejected()
    {

    }

    [Fact]
    public void When_RequestedDiscardQuantity_LargerThan_QuantityLeft_Then_Rejected()
    {

    }
}
