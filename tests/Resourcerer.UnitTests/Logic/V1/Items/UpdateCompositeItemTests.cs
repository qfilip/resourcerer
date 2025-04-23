using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.Exceptions;
using Resourcerer.Logic.Models;
using Resourcerer.Logic.V1;
using Resourcerer.UnitTests.Utilities;
using SqlForgery;

namespace Resourcerer.UnitTests.Logic.V1.Items;

public class UpdateCompositeItemTests : TestsBase
{
    private readonly UpdateCompositeItem.Handler _sut;
    public UpdateCompositeItemTests()
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
            ItemId = oldComposite.Id,
            CategoryId = _forger.Fake<Category>().Id,
            UnitOfMeasureId = _forger.Fake<UnitOfMeasure>().Id,
            Name = "test",
            ExpirationTimeSeconds = 10,
            ProductionPrice = 10,
            ProductionTimeSeconds = 10,
            UnitPrice = 10,
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
                    .Include(x => x.Prices)
                    .Include(x => x.Recipes)
                        .ThenInclude(x => x.RecipeExcerpts)
                    .Single(x => x.Id == request.ItemId);

                Assert.Equal(request.CategoryId, updatedComposite.CategoryId);
                Assert.Equal(request.UnitOfMeasureId, updatedComposite.UnitOfMeasureId);
                Assert.Equal(request.Name, updatedComposite.Name);
                Assert.Equal(request.ExpirationTimeSeconds, updatedComposite.ExpirationTimeSeconds);
                Assert.Equal(request.ProductionPrice, updatedComposite.ProductionPrice);
                Assert.Equal(request.ProductionTimeSeconds, updatedComposite.ProductionTimeSeconds);

                Assert.Equal(oldComposite.Prices.Count + 1, updatedComposite.Prices.Count);
                var latestPrice = updatedComposite.Prices.OrderBy(x => x.AuditRecord.CreatedAt).Last();
                Assert.Equal(request.UnitPrice, latestPrice.UnitValue);

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
            ItemId = Guid.NewGuid(),
            ExcerptMap = excerptMap
        };

        _ctx.SaveChanges();

        // act
        var result = _sut.Handle(request).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.NotFound, result.Status);
    }

    [Fact]
    public void CategoryNotFound__Rejected()
    {
        // arrange
        (var oldComposite, var excerptMap) = FakeData(_forger, _ctx);

        var request = new V1ChangeCompositeItemRecipe
        {
            ItemId = oldComposite.Id,
            ExcerptMap = excerptMap,
            CategoryId = Guid.NewGuid()
        };

        _ctx.SaveChanges();

        // act
        var result = _sut.Handle(request).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    [Fact]
    public void UnitOfMeasureNotFound__Rejected()
    {
        // arrange
        (var oldComposite, var excerptMap) = FakeData(_forger, _ctx);

        var request = new V1ChangeCompositeItemRecipe
        {
            ItemId = oldComposite.Id,
            ExcerptMap = excerptMap,
            CategoryId = oldComposite.CategoryId,
            UnitOfMeasureId = Guid.NewGuid()
        };

        _ctx.SaveChanges();

        // act
        var result = _sut.Handle(request).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    [Fact]
    public void CompositeHasNoRecipes__Rejected()
    {
        // arrange
        var request = new V1ChangeCompositeItemRecipe
        {
            ItemId = _forger.Fake<Item>().Id,
            ExcerptMap = new Dictionary<Guid, double>()
        };

        _ctx.SaveChanges();

        // act
        var result = _sut.Handle(request).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    [Fact]
    public void ElementItemsMissing__Rejected()
    {
        // arrange
        (var oldComposite, var excerptMap) = FakeData(_forger, _ctx);
        excerptMap.Add(Guid.NewGuid(), 1);

        var request = new V1ChangeCompositeItemRecipe
        {
            ItemId = oldComposite.Id,
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

            x.Prices = Enumerable.Range(1, 3)
                .Select(i => forger.Fake<Price>(r =>
                {
                    r.Item = x;
                    r.UnitValue = i + 1;
                    r.AuditRecord = new() { CreatedAt = new DateTime(2000, 1, i) };
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
