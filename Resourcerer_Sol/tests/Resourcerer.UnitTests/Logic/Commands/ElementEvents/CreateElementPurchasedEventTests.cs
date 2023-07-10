using Resourcerer.Dtos;
using Resourcerer.Logic;
using Resourcerer.Logic.Commands.ElementEvents;
using Resourcerer.UnitTests.Utilities.Mocker;

namespace Resourcerer.UnitTests.Logic.Commands.ElementEvents;

public class CreateElementPurchasedEventTests : TestsBase
{
    private readonly CreateElementPurchasedEvent.Handler _handler;
    public CreateElementPurchasedEventTests()
    {
        _handler = new(_testDbContext);
    }

    [Fact]
    public void When_AllOk_Then_Ok()
    {
        // arrange
        var element = Mocker.MockElement(_testDbContext);
        _testDbContext.SaveChanges();

        var dto = new CreateElementPurchasedEventDto
        {
            ElementId = element.Id,
            UnitPrice = 10,
            UnitsBought = 10,
            ExpectedDeliveryTime = DateTime.UtcNow,
            TotalDiscountPercent = 0
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
        var dto = new CreateElementPurchasedEventDto
        {
            ElementId = Guid.NewGuid(),
            UnitPrice = 10,
            UnitsBought = 10,
            ExpectedDeliveryTime = DateTime.UtcNow,
            TotalDiscountPercent = 0
        };

        // act
        var result = _handler.Handle(dto).GetAwaiter().GetResult();

        // assert
        Assert.Equal(eHandlerResultStatus.ValidationError, result.Status);
    }
}
