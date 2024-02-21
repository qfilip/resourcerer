using Resourcerer.Dtos.Instances.Events;
using Resourcerer.Logic;
using Resourcerer.Logic.V1_0.Commands;
using Resourcerer.UnitTests.Utilities;
using Resourcerer.UnitTests.Utilities.Faker;

namespace Resourcerer.UnitTests.Logic.V1_0.Commands.Instances;

public class CreateInstanceDiscardedEventTests : TestsBase
{
    public readonly CreateInstanceDiscardedEvent.Handler _handler;
    public CreateInstanceDiscardedEventTests()
    {
        _handler = new(_testDbContext);
    }

    [Fact]
    public void When_AllOk_Then_Ok()
    {
        // arrange
        var orderEvent = DF.FakeOrderedEvent(_testDbContext, x => x.DeliveredEvent = DF.FakeDeliveredEvent());
        var dto = new InstanceDiscardedRequestDto
        {
            InstanceId = orderEvent.DerivedInstanceId,
            Quantity = orderEvent.Quantity
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
        var dto = new InstanceDiscardedRequestDto
        {
            InstanceId = Guid.NewGuid()
        };

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.NotFound, result.Status);
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
