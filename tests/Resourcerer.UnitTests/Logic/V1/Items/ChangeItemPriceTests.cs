using FakeItEasy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Resourcerer.Application.Models;
using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Enums;
using Resourcerer.DataAccess.Utilities.Faking;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.V1.Items;
using Resourcerer.UnitTests.Utilities;

namespace Resourcerer.UnitTests.Logic.V1.Items;

public class ChangeItemPriceTests : TestsBase
{
    private readonly ChangeItemPrice.Handler _handler;
    private readonly ILogger<ChangeItemPrice.Handler> _fakeLogger;
    public ChangeItemPriceTests()
    {
        _fakeLogger = A.Fake<ILogger<ChangeItemPrice.Handler>>();
        _handler = new ChangeItemPrice.Handler(_ctx, new(), _fakeLogger);
    }

    [Fact]
    public void HappyPath__Ok()
    {
        // arrange
        var item = DF.Fake<Item>(_ctx);
        var price1 = DF.Fake<Price>(_ctx, x => x.Item = item);
        var price2 = DF.Fake<Price>(_ctx, x =>
        {
            x.Item = item;
            x.EntityStatus = eEntityStatus.Deleted;
        });

        var dto = new V1ChangePrice
        {
            ItemId = item.Id,
            UnitPrice = 20
        };

        _ctx.SaveChanges();

        // act
        var result = _handler.Handle(dto).Await();
        var newPrices = _ctx.Prices
            .Where(x => x.ItemId == item.Id)
            .IgnoreQueryFilters()
            .ToList();

        // assert
        Assert.Multiple(
            () => Assert.Equal(eHandlerResultStatus.Ok, result.Status),
            () =>
            {
                _ctx.Clear();

                var prices = _ctx.Prices
                    .Select(x => new { x.EntityStatus, x.UnitValue })
                    .IgnoreQueryFilters()
                    .ToArray();

                Assert.Single(prices.Where(x => x.EntityStatus == eEntityStatus.Active));
                Assert.Single(prices.Where(x => x.UnitValue == dto.UnitPrice));
            }
        );
    }

    [Fact]
    public void Element_NotExist_Then__NotFound()
    {
        // arrange
        var dto = new V1ChangePrice
        {
            ItemId = Guid.NewGuid(),
            UnitPrice = 20
        };

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.NotFound, result.Status);
    }

    [Fact]
    public void Element_HasCorruptedPrices_Then_PricesAreFixed__Ok()
    {
        // arrange
        var item = DF.Fake<Item>(_ctx);
        var oldPrices = Enumerable.Range(0, 3)
            .Select(_ => DF.Fake<Price>(_ctx, x => x.Item = item))
            .ToArray();

        var dto = new V1ChangePrice
        {
            ItemId = item.Id,
            UnitPrice = 20
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

                var dbOldPrices = _ctx.Prices
                    .Where(x => x.ItemId == item.Id)
                    .IgnoreQueryFilters()
                .ToArray();

                Assert.Equal(oldPrices.Length + 1, dbOldPrices.Length);
                Assert.Single(dbOldPrices.Where(x => x.EntityStatus == eEntityStatus.Active));
            }
        );
    }
}
