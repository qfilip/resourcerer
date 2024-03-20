using Resourcerer.Dtos.V1;
using Resourcerer.Logic;
using Resourcerer.Logic.V1.Commands;
using Resourcerer.UnitTests.Utilities;
using Resourcerer.UnitTests.Utilities.Faker;

namespace Resourcerer.UnitTests.Logic.V1_0;

public class CreateCompositeItemTests : TestsBase
{
    private readonly CreateCompositeItem.Handler _handler;
    public CreateCompositeItemTests()
    {
        _handler = new(_ctx);
    }

    [Fact]
    public void When_AllOk_Then_Ok()
    {
        // arrange
        var dto = GetDto();
        _ctx.SaveChanges();

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Ok, result.Status);
    }

    [Fact]
    public void When_ElementWithSameNameAndCategory_Exsts_Then_ValidationError()
    {
        // arrange
        var existingElement = DF.FakeItem(_ctx);
        var dto = GetDto(x =>
        {
            x.Name = existingElement.Name;
            x.CategoryId = existingElement.CategoryId;
        });
        _ctx.SaveChanges();

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    [Fact]
    public void When_Category_NotFound_Then_ValidationError()
    {
        // arrange
        var dto = GetDto(x => x.CategoryId = Guid.NewGuid());
        _ctx.SaveChanges();

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    [Fact]
    public void When_UnitOfMeasure_NotFound_Then_ValidationError()
    {
        // arrange
        var dto = GetDto(x => x.UnitOfMeasureId = Guid.NewGuid());
        _ctx.SaveChanges();

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    [Fact]
    public void When_RequiredElements_NotFound_Then_ValidationError()
    {
        // arrange
        var dto = GetDto(x => x.ExcerptMap = new Dictionary<Guid, double>
        {
            { Guid.NewGuid(), 1 },
            { Guid.NewGuid(), 2 }
        });
        _ctx.SaveChanges();

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    private V1CreateCompositeItem GetDto(Action<V1CreateCompositeItem>? modifier = null)
    {
        var dto = new V1CreateCompositeItem
        {
            Name = "test",
            CategoryId = DF.FakeCategory(_ctx).Id,
            UnitOfMeasureId = DF.FakeUnitOfMeasure(_ctx).Id,
            UnitPrice = 2,
            PreparationTimeSeconds = 2,
            ExpirationTimeSeconds = 2,
            ExcerptMap = new Dictionary<Guid, double>
            {
                { DF.FakeItem(_ctx).Id, 1 },
                { DF.FakeItem(_ctx).Id, 2 }
            }
        };

        modifier?.Invoke(dto);

        return dto;
    }
}
