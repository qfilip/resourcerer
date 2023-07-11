using FakeItEasy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos;
using Resourcerer.Logic;
using Resourcerer.Logic.Commands.Composites;
using Resourcerer.UnitTests.Utilities.Mocker;

namespace Resourcerer.UnitTests.Logic.Commands.Composites;

public class CreateCompositeTests : TestsBase
{
    private readonly CreateComposite.Handler _handler;
    public CreateCompositeTests()
    {
        _handler = new CreateComposite.Handler(_testDbContext, A.Fake<ILogger<CreateComposite.Handler>>());
    }

    [Fact]
    public void When_RequiredElements_Exists_Then_Ok()
    {
        // arrange
        var requiredElements = SeedRequiredElements();
        var dto = GetDto(requiredElements);
        _testDbContext.SaveChanges();

        // act
        var result = _handler.Handle(dto).GetAwaiter().GetResult();
        
        var entity = _testDbContext.Composites
            .Include(x => x.Excerpts)
            .Include(x => x.Prices)
            .Single();

        // assert
        Assert.Equal(eHandlerResultStatus.Ok, result.Status);
        Assert.Equal(dto.CategoryId, entity.CategoryId);
        Assert.Equal(dto.UnitPrice, entity.Prices.Single().UnitValue);
        Assert.True(entity.Excerpts.All(x =>
        {
            return dto.Elements!
                .Single(e => e.ElementId == x.ElementId && e.Quantity == x.Quantity) != null;
        }));
    }

    [Fact]
    public void When_Composite_WithSameName_Exists_Then_ValidationError()
    {
        // arrange
        var existing = Mocker.MockComposite(_testDbContext);
        var requiredElements = SeedRequiredElements();
        var dto = GetDto(requiredElements, x => x.Name = existing.Name);
        _testDbContext.SaveChanges();

        // act
        var result = _handler.Handle(dto).GetAwaiter().GetResult();
        
        // assert
        Assert.Equal(eHandlerResultStatus.ValidationError, result.Status);
    }

    [Fact]
    public void When_RequestedCategory_NotExists_Then_ValidationError()
    {
        // arrange
        var requiredElements = SeedRequiredElements();
        var dto = GetDto(requiredElements, x => x.CategoryId = Guid.Empty);
        _testDbContext.SaveChanges();

        // act
        var result = _handler.Handle(dto).GetAwaiter().GetResult();

        // assert
        Assert.Equal(eHandlerResultStatus.ValidationError, result.Status);
    }

    [Fact]
    public void When_RequestedUnitOfMeasure_NotExists_Then_ValidationError()
    {
        // arrange
        var requiredElements = SeedRequiredElements();
        var dto = GetDto(requiredElements, x => x.UnitOfMeasureId = Guid.Empty);
        _testDbContext.SaveChanges();

        // act
        var result = _handler.Handle(dto).GetAwaiter().GetResult();

        // assert
        Assert.Equal(eHandlerResultStatus.ValidationError, result.Status);
    }

    [Fact]
    public void When_AllRequiredElements_NotExist_Then_ValidationError()
    {
        // arrange
        var requiredElements = SeedRequiredElements();
        var dto = GetDto(requiredElements, x => x.Elements!.Add(new CompositeElementsDto
        {
            ElementId = Guid.NewGuid(),
            Quantity = 6
        }));
        _testDbContext.SaveChanges();

        // act
        var result = _handler.Handle(dto).GetAwaiter().GetResult();

        // assert
        Assert.Equal(eHandlerResultStatus.ValidationError, result.Status);
    }

    [Fact]
    public void When_ElementWithNoPriceUsed_Then_ValidationError()
    {
        // arrange
        var requiredElements = SeedRequiredElements(0, false);
        var dto = GetDto(requiredElements);
        _testDbContext.SaveChanges();

        // act
        var result = _handler.Handle(dto).GetAwaiter().GetResult();

        // assert
        Assert.Equal(eHandlerResultStatus.ValidationError, result.Status);
    }

    [Fact]
    public void When_RequiredElementHasSeveralActivePrices_Then_ValidationError()
    {
        // arrange
        var requiredElements = SeedRequiredElements(3, true);
        var dto = GetDto(requiredElements);
        _testDbContext.SaveChanges();

        // act
        var result = _handler.Handle(dto).GetAwaiter().GetResult();

        // assert
        Assert.Equal(eHandlerResultStatus.ValidationError, result.Status);
    }

    private Item[] SeedRequiredElements(int priceCount = 3, bool pricesCorrupted = false)
    {
        return new Item[]
        {
            Mocker.MockElement(_testDbContext, priceCount, pricesCorrupted),
            Mocker.MockElement(_testDbContext, priceCount, pricesCorrupted),
        };
    }

    private CreateCompositeDto GetDto(
        Item[] requiredElements,
        Action<CreateCompositeDto>? modifier = null)
    {
        var dto = new CreateCompositeDto
        {
            Name = "test",
            CategoryId = Mocker.MockCategory(_testDbContext).Id,
            UnitOfMeasureId = Mocker.MockUnitOfMeasure(_testDbContext).Id,
            UnitPrice = 10,
            Elements = requiredElements
                .Select(x => new CompositeElementsDto
                {
                    ElementId = x.Id,
                    Quantity = 5
                })
                .ToList()
        };

        modifier?.Invoke(dto);

        return dto;
    }
}
