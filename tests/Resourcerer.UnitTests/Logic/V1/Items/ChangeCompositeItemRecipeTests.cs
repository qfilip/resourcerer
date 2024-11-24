using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.Exceptions;
using Resourcerer.Logic.Models;
using Resourcerer.Logic.V1;
using Resourcerer.UnitTests.Utilities;
using SqlForgery;

namespace Resourcerer.UnitTests.Logic.V1.Items;

public class ChangeCompositeItemRecipeTests : TestsBase
{
    private readonly ChangeCompositeItemRecipe.Handler _sut;
    public ChangeCompositeItemRecipeTests()
    {
        _sut = new(_ctx, new(), GetMapster());
    }

    [Fact]
    public void HappyPath__Ok()
    {
        // arrange
        (var oldComposite, var excerptMap) = FakeData(_forger, _ctx);

        var request = new V1ChangeCompositeItemRecipe
        {
            CompositeId = oldComposite.Id,
            ExcerptMap = excerptMap
        };

        _ctx.SaveChanges();

        // act
        var result = _sut.Handle(request).Await();

        // assert
        Assert.Multiple(
            () => Assert.Equal(eHandlerResultStatus.Ok, result.Status),
            () =>
            {
                var updatedComposite = _ctx.Items
                    .Include(x => x.Recipes)
                        .ThenInclude(x => x.RecipeExcerpts)
                    .Single(x => x.Id == request.CompositeId);

                Assert.Equal(oldComposite.Recipes.Count + 1, updatedComposite.Recipes.Count);

                var recipeVersions = updatedComposite.Recipes
                    .Select(x => x.Version)
                    .ToArray();

                Assert.Equal(recipeVersions, recipeVersions.Distinct());

                var latestRecipe = updatedComposite.Recipes
                    .OrderByDescending(x => x.Version)
                    .First();

                foreach(var excerpt in latestRecipe.RecipeExcerpts)
                {
                    Assert.True(excerptMap.ContainsKey(excerpt.ElementId));
                }
            }
        );
    }

    [Fact]
    public void CompositeNotFound__NotFound()
    {
        // arrange
        (var oldComposite, var excerptMap) = FakeData(_forger, _ctx);

        var request = new V1ChangeCompositeItemRecipe
        {
            CompositeId = Guid.NewGuid(),
            ExcerptMap = excerptMap
        };

        _ctx.SaveChanges();

        // act
        var result = _sut.Handle(request).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.NotFound, result.Status);
    }

    [Fact]
    public void CompositeHasNoRecipes__Exception()
    {
        // arrange
        var request = new V1ChangeCompositeItemRecipe
        {
            CompositeId = _forger.Fake<Item>().Id,
            ExcerptMap = new Dictionary<Guid, double>()
        };

        _ctx.SaveChanges();

        var handler = () => _sut.Handle(request);
        
        // act, assert
        Assert.ThrowsAsync<DataCorruptionException>(handler);
    }

    [Fact]
    public void ElementItemsMissing__Rejected()
    {
        // arrange
        (var oldComposite, var excerptMap) = FakeData(_forger, _ctx);
        excerptMap.Add(Guid.NewGuid(), 1);

        var request = new V1ChangeCompositeItemRecipe
        {
            CompositeId = oldComposite.Id,
            ExcerptMap = excerptMap
        };

        _ctx.SaveChanges();

        // act
        var result = _sut.Handle(request).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    private static (Item, Dictionary<Guid, double>) FakeData(Forger forger, TestDbContext ctx)
    {
        var composite = forger.Fake<Item>(x =>
        {
            x.Recipes = Enumerable.Range(0, 2)
                .Select(i => forger.Fake<Recipe>(r =>
                {
                    r.CompositeItem = x;
                    r.Version = i + 1;
                }))
                .ToArray();
        });

        var excerptMap = Enumerable.Range(0, 2)
            .Select(_ => new
            {
                forger.Fake<Item>().Id,
                Quantity = 1d
            })
            .ToDictionary(
                x => x.Id,
                x => x.Quantity
            );

        ctx.SaveChanges();

        return (composite, excerptMap);
    }
}
