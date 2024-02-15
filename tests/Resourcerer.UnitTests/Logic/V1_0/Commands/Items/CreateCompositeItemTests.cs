using Resourcerer.Dtos;
using Resourcerer.Logic;
using Resourcerer.Logic.Commands.V1_0;
using Resourcerer.UnitTests.Utilities;
using Resourcerer.UnitTests.Utilities.Faker;

namespace Resourcerer.UnitTests.Logic.V1_0;

public class CreateCompositeItemTests : TestsBase
{
    private readonly CreateCompositeItem.Handler _handler;
    public CreateCompositeItemTests()
    {
        _handler = new(_testDbContext);
    }

    [Fact]
    public void When_AllOk_Then_Ok()
    {
        // arrange
        var dto = GetDto();
        _testDbContext.SaveChanges();

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Ok, result.Status);
    }

    [Fact]
    public void When_ElementWithSameName_Exsts_Then_ValidationError()
    {
        // arrange
        var existingElement = DF.FakeItem(_testDbContext);
        var dto = GetDto(x => x.Name = existingElement.Name);
        _testDbContext.SaveChanges();

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
        _testDbContext.SaveChanges();

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
        _testDbContext.SaveChanges();

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
        _testDbContext.SaveChanges();

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    private CreateCompositeItemDto GetDto(Action<CreateCompositeItemDto>? modifier = null)
    {
        var dto = new CreateCompositeItemDto
        {
            Name = "test",
            CategoryId = DF.FakeCategory(_testDbContext).Id,
            UnitOfMeasureId = DF.FakeUnitOfMeasure(_testDbContext).Id,
            UnitPrice = 2,
            PreparationTimeSeconds = 2,
            ExpirationTimeSeconds = 2,
            ExcerptMap = new Dictionary<Guid, double>
            {
                { DF.FakeItem(_testDbContext).Id, 1 },
                { DF.FakeItem(_testDbContext).Id, 2 }
            }
        };

        modifier?.Invoke(dto);

        return dto;
    }
}
