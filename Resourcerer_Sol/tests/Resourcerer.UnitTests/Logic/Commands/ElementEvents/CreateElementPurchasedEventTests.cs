using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos;
using Resourcerer.Logic.Commands.ElementEvents;

namespace Resourcerer.UnitTests.Logic.Commands.ElementEvents;

public class CreateElementPurchasedEventTests : TestsBase
{
    private readonly CreateElementPurchasedEvent.Handler _handler;
    public CreateElementPurchasedEventTests()
    {
        _handler = new(_testDbContext);
    }

    [Fact]
    public void When_ElementExists_Then_Ok()
    {
        // arrange
        var element = Mocker.MockLE
        var dto = new CreateElementPurchasedEventDto
        {
            ElementId
        };

        // act
        var result = _handler.Handle(dto).GetAwaiter().GetResult();
    }

    [Fact]
    public void When_ElementNotExists_Then_ValidationError() { }
}
