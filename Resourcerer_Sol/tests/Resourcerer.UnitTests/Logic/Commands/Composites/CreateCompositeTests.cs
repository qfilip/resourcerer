using FakeItEasy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Resourcerer.Dtos;
using Resourcerer.Logic;
using Resourcerer.Logic.Commands.Composites;

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
        var requiredElements = new[]
        {
            Mocker.MockElement(_testDbContext),
            Mocker.MockElement(_testDbContext),
        };

        var dto = new CreateCompositeDto
        {
            Name = "test",
            CategoryId = Mocker.MockCategory(_testDbContext).Id,
            UnitPrice = 10,
            Elements = requiredElements
                .Select(x => new CompositeElementsDto
                {
                    ElementId = x.Id,
                    Quantity = 5
                })
                .ToList()
        };

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
            return dto.Elements
                .All(e => e.ElementId == x.ElementId && e.Quantity == x.Quantity);
        }));
    }

    [Fact]
    public void When_Composite_WithSameName_Exists_Then_ValidationError()
    {

    }

    [Fact]
    public void When_RequestedCategory_NotExists_Then_ValidationError()
    {

    }

    [Fact]
    public void When_AllRequiredElements_NotExist_Then_ValidationError()
    {

    }

    [Fact]
    public void When_ElementWithNoPriceUsed_Then_ValidationError()
    {

    }

    [Fact]
    public void When_RequiredElementHasSeveralActivePrices_Then_ValidationError()
    {

    }
}
