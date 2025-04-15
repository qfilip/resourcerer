using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.Models;
using Resourcerer.Logic.V1;
using Resourcerer.Logic.V1.Items;
using Resourcerer.UnitTests.Utilities;

namespace Resourcerer.UnitTests.Logic.V1.Items;

public class UpdateElementItemTests : TestsBase
{
    private readonly UpdateElementItem.Handler _handler;
    public UpdateElementItemTests()
    {
        _handler = new UpdateElementItem.Handler(_ctx, _mapper, new());
    }

    [Fact]
    public void HappyPath__Ok()
    {
        // arrange
        var category = _forger.Fake<Category>();
        var uom = _forger.Fake<UnitOfMeasure>();
        var item = FakeItem();
        var dto = new V1UpdateElementItem
        {
            ItemId = item.Id,
            Name = "test",
            CategoryId = category.Id,
            UnitOfMeasureId = uom.Id,
            ProductionPrice = 17,
            ProductionTimeSeconds = 17,
            ExpirationTimeSeconds = 17,
            UnitPrice = 2
        };
        _ctx.SaveChanges();

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Multiple(
            () => Assert.Equal(eHandlerResultStatus.Ok, result.Status),
            () =>
            {
                _ctx.Clear();
                var updatedItem = _ctx.Items.First(x => x.Id == item.Id);
                var prices = _ctx.Prices.Where(x => x.ItemId == item.Id).ToArray();
                var currentPrice = prices.OrderByDescending(x => x.AuditRecord.CreatedAt).Last();
                
                Assert.Equal(dto.Name, updatedItem.Name);
                Assert.Equal(dto.CategoryId, updatedItem.CategoryId);
                Assert.Equal(dto.UnitOfMeasureId, updatedItem.UnitOfMeasureId);
                Assert.Equal(dto.ProductionPrice, updatedItem.ProductionPrice);
                Assert.Equal(dto.ProductionTimeSeconds, updatedItem.ProductionTimeSeconds);
                Assert.Equal(dto.ExpirationTimeSeconds, updatedItem.ExpirationTimeSeconds);
                
                Assert.True(prices.Length == 1);
                Assert.NotNull(currentPrice);
                Assert.Equal(dto.UnitPrice, currentPrice!.UnitValue);
            }
        );
    }

    [Fact]
    public void ItemNotFound__NotFound()
    {
        // arrange
        var existingItem = _forger.Fake<Item>();
        var dto = new V1UpdateElementItem
        {
            ItemId = Guid.NewGuid(),
            Name = existingItem.Name,
            CategoryId = existingItem.CategoryId,
            UnitOfMeasureId = existingItem.UnitOfMeasure!.Id,
            UnitPrice = 2
        };
        _ctx.SaveChanges();

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.NotFound, result.Status);
    }

    [Fact]
    public void Category_NotFound__Rejected()
    {
        // arrange
        var comp = _forger.Fake<Company>();
        var uom = _forger.Fake<UnitOfMeasure>();
        var item = FakeItem();
        var dto = new V1UpdateElementItem
        {
            ItemId = item.Id,
            Name = "test",
            CategoryId = Guid.NewGuid(),
            UnitOfMeasureId = uom.Id,
            UnitPrice = 2
        };
        _ctx.SaveChanges();

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    [Fact]
    public void UnitOfMeasure_NotFound__Rejected()
    {
        // arrange
        var category = _forger.Fake<Category>();
        var item = FakeItem();
        var dto = new V1UpdateElementItem
        {
            ItemId = item.Id,
            Name = "test",
            CategoryId = category.Id,
            UnitOfMeasureId = Guid.NewGuid(),
            UnitPrice = 2
        };
        _ctx.SaveChanges();

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    private Item FakeItem()
    {
        return _forger.Fake<Item>(x =>
        {
            x.Category = _forger.Fake<Category>();
            x.UnitOfMeasure = _forger.Fake<UnitOfMeasure>();
        });
    }
}
