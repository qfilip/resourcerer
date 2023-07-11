using Resourcerer.Dtos;
using Resourcerer.Logic;
using Resourcerer.Logic.Commands.Events;
using Resourcerer.UnitTests.Utilities.Mocker;

namespace Resourcerer.UnitTests.Logic.Commands.ElementEvents;

public class CreateInstanceOrderedEventTests : TestsBase
{
    private readonly CreateInstanceOrderedEvent.Handler _handler;
    public CreateInstanceOrderedEventTests()
    {
        _handler = new(_testDbContext);
    }

    [Fact]
    public void When_AllOk_Then_Ok()
    {
        // arrange
        var element = Mocker.MockItem(_testDbContext);
        _testDbContext.SaveChanges();

        var dto = new InstanceOrderedEventDto
        {
            ElementId = element.Id,
            UnitPrice = 10,
            UnitsOrdered = 10,
            ExpectedDeliveryDate = DateTime.UtcNow,
            TotalDiscountPercent = 2
        };

        // act
        var result = _handler.Handle(dto).GetAwaiter().GetResult();

        // assert
        Assert.Equal(eHandlerResultStatus.Ok, result.Status);
    }

    [Fact]
    public void When_ElementNotExists_Then_ValidationError()
    {
        // arrange
        var dto = new InstanceOrderedEventDto
        {
            ElementId = Guid.NewGuid(),
            UnitPrice = 10,
            UnitsOrdered = 10,
            ExpectedDeliveryDate = DateTime.UtcNow,
            TotalDiscountPercent = 0
        };

        // act
        var result = _handler.Handle(dto).GetAwaiter().GetResult();

        // assert
        Assert.Equal(eHandlerResultStatus.ValidationError, result.Status);
    }

    [Fact]
    public void When_ExpirationDateInvalid_Then_ValidationError()
    {
        // arrange
        var dto = new InstanceOrderedEventDto
        {
            ElementId = Guid.NewGuid(),
            UnitPrice = 10,
            UnitsOrdered = 10,
            ExpectedDeliveryDate = DateTime.UtcNow,
            TotalDiscountPercent = 0
        };

        // act
        var result = _handler.Handle(dto).GetAwaiter().GetResult();

        // assert
        Assert.Equal(eHandlerResultStatus.ValidationError, result.Status);
    }
}
