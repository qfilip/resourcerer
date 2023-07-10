using Castle.Core.Logging;
using FakeItEasy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Resourcerer.DataAccess.Enums;
using Resourcerer.Dtos;
using Resourcerer.Logic;
using Resourcerer.Logic.Commands.Elements;
using Resourcerer.UnitTests.Utilities.Mocker;
using SQLitePCL;

namespace Resourcerer.UnitTests.Logic.Commands.Elements;

public class ChangeElementPriceTests : TestsBase
{
    private readonly ChangeElementPrice.Handler _handler;
    private readonly ILogger<ChangeElementPrice.Handler> _fakeLogger;
    public ChangeElementPriceTests()
    {
        _fakeLogger = A.Fake<ILogger<ChangeElementPrice.Handler>>();
        _handler = new ChangeElementPrice.Handler(_testDbContext, _fakeLogger);
    }

    [Fact]
    public void When_AllOk_Then_NewPriceAdded_Ok()
    {
        // arrange
        var element = Mocker.MockElement(_testDbContext);
        var oldPrices = element.Prices.Select(x => x).ToList();
        var dto = new ChangePriceDto
        {
            EntityId = element.Id,
            UnitPrice = 20
        };
        _testDbContext.SaveChanges();

        // act
        var result = _handler.Handle(dto).GetAwaiter().GetResult();
        var newPrices = _testDbContext.Prices
            .Where(x => x.ElementId == element.Id)
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
        var dto = new ChangePriceDto
        {
            EntityId = Guid.NewGuid(),
            UnitPrice = 20
        };
        _testDbContext.SaveChanges();

        // act
        var result = _handler.Handle(dto).GetAwaiter().GetResult();

        // assert
        Assert.Equal(eHandlerResultStatus.ValidationError, result.Status);
    }

    [Fact]
    public void When_Element_HasCorruptedPrices_Then_PricesAreFixed_Ok()
    {
        // arrange
        var element = Mocker.MockElement(_testDbContext, 3, true);
        var oldPrices = element.Prices.Select(x => x).ToList();
        var dto = new ChangePriceDto
        {
            EntityId = element.Id,
            UnitPrice = 20
        };
        _testDbContext.SaveChanges();

        // act
        var result = _handler.Handle(dto).GetAwaiter().GetResult();
        var newPrices = _testDbContext.Prices
            .Where(x => x.ElementId == element.Id)
            .IgnoreQueryFilters()
            .ToList();

        // assert
        Assert.Equal(eHandlerResultStatus.Ok, result.Status);
        Assert.Equal(oldPrices.Count + 1, newPrices.Count);
        Assert.Equal(1, newPrices.Count(x => x.EntityStatus == eEntityStatus.Active));
    }
}
