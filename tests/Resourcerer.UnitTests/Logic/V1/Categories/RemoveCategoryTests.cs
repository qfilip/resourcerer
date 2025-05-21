using Resourcerer.Logic.Models;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.Entities;
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
        var children = Enumerable.Range(1, 10).
            Select(_ => _forger.Fake<Category>(x =>
            {
                x.ParentCategory = category;
            })).ToArray();

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
                var deletedCategoryCount = _ctx.Categories
                    .Where(x => x.Id == category.Id)
                    .Count();
                
                Assert.Equal(0, deletedCategoryCount);

                var parentlessCategoryCount = _ctx.Categories
                    .Where(x => x.ParentCategoryId == null)
                    .Count();

                Assert.Equal(children.Length, parentlessCategoryCount);
            }
        );
    }

    [Fact]
    public void CategoryNotFound__NotFound()
    {
        // arrange
        var category = _forger.Fake<Category>();
        var dto = new CategoryDto { Id = Guid.NewGuid() };
        _ctx.SaveChanges();

        // act
        var result = _sut.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.NotFound, result.Status);
    }
}
