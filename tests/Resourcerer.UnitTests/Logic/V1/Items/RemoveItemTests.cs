using Resourcerer.DataAccess.Entities;
using Resourcerer.Logic.Models;
using Resourcerer.Logic.V1;
using Resourcerer.UnitTests.Utilities;

namespace Resourcerer.UnitTests.Logic.V1.Items;

public class RemoveItemTests : TestsBase
{
    private readonly RemoveItem.Handler _sut;
    public RemoveItemTests()
    {
        _sut = new(_ctx);
    }

    [Fact]
    public void WithoutProductionOrders__Ok()
    {
        // arrange
        var item = _forger.Fake<Item>();
        _ctx.SaveChanges();

        // act
        var result = _sut.Handle(item.Id).Await();

        // assert
        Assert.Multiple(
            () => Assert.Equal(eHandlerResultStatus.Ok, result.Status),
            () =>
            {
                var count = _ctx.Items.Where(x => x.Id == item.Id).Count();
                Assert.Equal(0, count);
            }
        );
    }

    [Fact]
    public void WithCancelledProductionOrderResolved__Ok()
    {
        // arrange
        var item = _forger.Fake<Item>();
        var order = _forger.Fake<ItemProductionOrder>(x =>
        {
            x.Item = item;
            x.CancelledEvent = new();
        });

        _ctx.SaveChanges();

        // act
        var result = _sut.Handle(item.Id).Await();

        // assert
        Assert.Multiple(
            () => Assert.Equal(eHandlerResultStatus.Ok, result.Status),
            () =>
            {
                var count = _ctx.Items.Where(x => x.Id == item.Id).Count();
                Assert.Equal(0, count);
            }
        );
    }

    [Fact]
    public void WithFinishedProductionOrderResolved__Ok()
    {
        // arrange
        var item = _forger.Fake<Item>();
        var order = _forger.Fake<ItemProductionOrder>(x =>
        {
            x.Item = item;
            x.FinishedEvent = new();
        });

        _ctx.SaveChanges();

        // act
        var result = _sut.Handle(item.Id).Await();

        // assert
        Assert.Multiple(
            () => Assert.Equal(eHandlerResultStatus.Ok, result.Status),
            () =>
            {
                var count = _ctx.Items.Where(x => x.Id == item.Id).Count();
                Assert.Equal(0, count);
            }
        );
    }

    [Fact]
    public void WithUnresolvedProductionOrders__Rejected()
    {
        // arrange
        var item = _forger.Fake<Item>();
        var finished = _forger.Fake<ItemProductionOrder>(x =>
        {
            x.Item = item;
            x.FinishedEvent = new();
        });
        var cancelled = _forger.Fake<ItemProductionOrder>(x =>
        {
            x.Item = item;
            x.CancelledEvent = new();
        });
        var unresolved = _forger.Fake<ItemProductionOrder>(x =>
        {
            x.Item = item;
        });

        _ctx.SaveChanges();

        // act
        var result = _sut.Handle(item.Id).Await();

        // assert
        Assert.Multiple(
            () => Assert.Equal(eHandlerResultStatus.Rejected, result.Status),
            () =>
            {
                var count = _ctx.Items.Where(x => x.Id == item.Id).Count();
                Assert.Equal(1, count);
            }
        );
    }

    [Fact]
    public void NotFound__NotFound()
    {
        // arrange
        var item = _forger.Fake<Item>();
        _ctx.SaveChanges();

        // act
        var result = _sut.Handle(Guid.NewGuid()).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.NotFound, result.Status);
    }
}
