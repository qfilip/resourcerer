using Resourcerer.Application.Models;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.Entity;
using Resourcerer.Logic.V1;
using Resourcerer.UnitTests.Utilities;

namespace Resourcerer.UnitTests.Logic.V1.Categories;

public class RemoveCategoryTests : TestsBase
{
    private readonly RemoveCategory.Handler _sut;
    public RemoveCategoryTests()
    {
        _sut = new RemoveCategory.Handler(_ctx, new());
    }

    [Fact]
    public void HappyPath__Ok()
    {
        // arrange
        var category = _forger.Fake<Category>();
        var dto = new CategoryDto { Id = category.Id };

        _ctx.SaveChanges();

        // act
        var result = _sut.Handle(dto).Await();

        // assert
        Assert.Multiple(
            () => Assert.Equal(eHandlerResultStatus.Ok, result.Status),
            () =>
            {
                _ctx.Clear();
                Assert.Equal(0, _ctx.Categories.Count());
            }
        );
    }

    [Fact]
    public void Validator__Ok()
    {
        var dto = new CategoryDto { Id = Guid.Empty };
        var result = _sut.Handle(dto).Await();
        Assert.Single(result.Errors);
    }
}
