using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos;
using Resourcerer.Logic;
using Resourcerer.Logic.Commands.ElementEvents;
using Resourcerer.UnitTests.Utilities;

namespace Resourcerer.UnitTests.Logic.Commands.ElementEvents;

public class CreateElementDeliveredEventTests
{
    private readonly AppDbContext _testDbContext;
    private readonly ElementPurchasedEvent _testPurchasedEvent;
    private readonly CreateElementDeliveredEvent.Handler _handler;
    public CreateElementDeliveredEventTests()
    {
        _testDbContext = new ContextCreator().GetTestDbContext();
        _testPurchasedEvent = PrepareData(_testDbContext);
        _handler = new CreateElementDeliveredEvent.Handler(_testDbContext);
    }

    [Fact]
    public async Task Ok_ElementInstanceCreated_When_ElementPurchasedEvent_Exists()
    {
        // arrange
        _testDbContext.ElementPurchasedEvents.Add(_testPurchasedEvent);
        await _testDbContext.SaveChangesAsync();

        var dto = new CreateElementDeliveredEventDto
        {
            ElementPurchasedEventId = _testPurchasedEvent.Id
        };

        // act
        var result = await _handler.Handle(dto);

        // assert
        Assert.Equal(eHandlerResultStatus.Ok, result.Status);
        await _testDbContext.Instances
            .FirstAsync(x => x.ElementId == _testPurchasedEvent.ElementId);
    }

    [Fact]
    public async Task ValidationError_When_ElementPurchaseCancelledEvent_Exists()
    {
        // arrange
        _testDbContext.ElementPurchasedEvents.Add(_testPurchasedEvent);
        _testDbContext.ElementPurchaseCancelledEvents.Add(new()
        {
            ElementPurchasedEventId = _testPurchasedEvent.Id
        });
        await _testDbContext.SaveChangesAsync();

        var dto = new CreateElementDeliveredEventDto()
        {
            ElementPurchasedEventId = _testPurchasedEvent.Id
        };

        // act
        var result = await _handler.Handle(dto);

        // assert
        Assert.Equal(eHandlerResultStatus.ValidationError, result.Status);
    }

    [Fact]
    public async Task IsIdempotent()
    {
        // arrange
        _testDbContext.ElementPurchasedEvents.Add(_testPurchasedEvent);
        await _testDbContext.SaveChangesAsync();

        var dto1 = new CreateElementDeliveredEventDto
        {
            ElementPurchasedEventId = _testPurchasedEvent.Id
        };
        var dto2 = new CreateElementDeliveredEventDto
        {
            ElementPurchasedEventId = _testPurchasedEvent.Id
        };

        // act
        var results = new[]
        {
            await _handler.Handle(dto1),
            await _handler.Handle(dto2)
        };

        // assert
        results.Every(x => Assert.Equal(eHandlerResultStatus.Ok, x.Status));
    }

    private static ElementPurchasedEvent PrepareData(AppDbContext ctx)
    {
        var uom = new UnitOfMeasure
        {
            Id = Guid.NewGuid(),
            Name = "foo",
            Symbol = "f"
        };
        var element = new Element
        {
            Id = Guid.NewGuid(),
            Name = "Foo",
            UnitOfMeasureId = uom.Id
        };
        ctx.UnitsOfMeasure.Add(uom);
        ctx.Elements.Add(element);
        ctx.BaseSaveChangesAsync().GetAwaiter().GetResult();

        return new()
        {
            ElementId = element.Id
        };
    }
}
