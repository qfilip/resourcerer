﻿using FakeItEasy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Resourcerer.DataAccess.Enums;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic;
using Resourcerer.Logic.V1.Commands;
using Resourcerer.UnitTests.Utilities;
using Resourcerer.UnitTests.Utilities.Faker;

namespace Resourcerer.UnitTests.Logic.V1.Commands.Items;

public class ChangeItemPriceTests : TestsBase
{
    private readonly ChangeItemPrice.Handler _handler;
    private readonly ILogger<ChangeItemPrice.Handler> _fakeLogger;
    public ChangeItemPriceTests()
    {
        _fakeLogger = A.Fake<ILogger<ChangeItemPrice.Handler>>();
        _handler = new ChangeItemPrice.Handler(_ctx, _fakeLogger);
    }

    [Fact]
    public void When_AllOk_Then_NewPriceAdded_Ok()
    {
        // arrange
        var item = DF.FakeItem(_ctx);
        _ctx.SaveChanges();
        
        var oldPrices = item.Prices.Select(x => x).ToList();
        var dto = new V1ChangePrice
        {
            ItemId = item.Id,
            UnitPrice = 20
        };

        // act
        var result = _handler.Handle(dto).Await();
        var newPrices = _ctx.Prices
            .Where(x => x.ItemId == item.Id)
            .IgnoreQueryFilters()
            .ToList();

        // assert
        Assert.Equal(eHandlerResultStatus.Ok, result.Status);
        Assert.Equal(oldPrices.Count + 1, newPrices.Count);
        Assert.Equal(1, newPrices.Count(x => x.EntityStatus == eEntityStatus.Active));
    }

    [Fact]
    public void When_Element_NotExist_Then_ValidationError()
    {
        // arrange
        var dto = new V1ChangePrice
        {
            ItemId = Guid.NewGuid(),
            UnitPrice = 20
        };
        _ctx.SaveChanges();

        // act
        var result = _handler.Handle(dto).Await();

        // assert
        Assert.Equal(eHandlerResultStatus.Rejected, result.Status);
    }

    [Fact]
    public void When_Element_HasCorruptedPrices_Then_PricesAreFixed_Ok()
    {
        // arrange
        var item = DF.FakeItem(_ctx, null, 1, 3, true);
        var oldPrices = item.Prices.Select(x => x).ToList();
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
        Assert.Equal(eHandlerResultStatus.Ok, result.Status);
        Assert.Equal(oldPrices.Count + 1, newPrices.Count);
        Assert.Equal(1, newPrices.Count(x => x.EntityStatus == eEntityStatus.Active));
    }
}