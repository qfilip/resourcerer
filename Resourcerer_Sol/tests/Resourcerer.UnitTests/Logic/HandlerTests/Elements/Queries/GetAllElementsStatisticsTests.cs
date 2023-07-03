using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.Elements;
using Resourcerer.Logic;
using Resourcerer.Logic.Queries.Elements;
using Resourcerer.UnitTests.Utilities;
using Resourcerer.UnitTests.Utilities.TestDatabaseMocks;

namespace Resourcerer.UnitTests.Logic.HandlerTests.Elements.Queries;

public class GetAllElementsStatisticsTests
{
    private readonly IAppDbContext _testDbContext;
    private readonly GetAllElementsStatistics.Handler _handler;

    public GetAllElementsStatisticsTests()
    {
        _testDbContext = new ContextCreator(CarpenterDbMocker.SeedAsync).GetTestDbContext();
        _handler = new GetAllElementsStatistics.Handler(_testDbContext);
    }

    [Fact]
    public async Task CorrectlySums_UsageDetails_When_ElementsArePurchased()
    {
        // arrange
        var (glass, metal) = GetGlassAndMetal(_testDbContext);
        var purchases = GetTestElementPurchases(glass, metal);

        _testDbContext.ElementPurchasedEvents.AddRange(purchases);
        await _testDbContext.BaseSaveChangesAsync();

        // assert
        var hResult = await _handler.Handle(new Unit());

        Assert.Equal(HandlerResultStatus.Ok, hResult.Status);

        var glassStats = hResult.Object!.First(x => x.ElementName == "glass");
        var metalStats = hResult.Object!.First(x => x.ElementName == "metal");

        AssertStats(glassStats, unitsUsedInComposites: 0, unitsInStock: 20, unitsPurchased: 20, purchaseCosts: 20);
        AssertStats(metalStats, unitsUsedInComposites: 0, unitsInStock: 20, unitsPurchased: 20, purchaseCosts: 40);
    }

    [Fact]
    public async void CorrectlySumsUsageDetails_When_ElementIsSold()
    {
        // arrange
        var (glass, metal) = GetGlassAndMetal(_testDbContext);
        var purchases = GetTestElementPurchases(glass, metal);

        var sales = purchases.Select(x => new ElementSoldEvent
        {
            ElementId = x.ElementId,
            UnitPrice = x.UnitPrice,
            UnitsSold = x.UnitsBought,
            UnitOfMeasure = x.UnitOfMeasure
        });

        _testDbContext.ElementPurchasedEvents.AddRange(purchases);
        _testDbContext.ElementSoldEvents.AddRange(sales);
        await _testDbContext.BaseSaveChangesAsync();

        // assert
        var hResult = await _handler.Handle(new Unit());

        Assert.Equal(HandlerResultStatus.Ok, hResult.Status);

        var glassStats = hResult.Object!.First(x => x.ElementName == "glass");
        var metalStats = hResult.Object!.First(x => x.ElementName == "metal");

        AssertStats(glassStats, unitsUsedInComposites: 0, unitsInStock: 0, unitsPurchased: 20, purchaseCosts: 20);
        AssertStats(metalStats, unitsUsedInComposites: 0, unitsInStock: 0, unitsPurchased: 20, purchaseCosts: 40);
    }

    [Fact]
    public async void CorrectlySumsUsageDetails_When_CompositeIsSold()
    {
        // arrange
        var window = _testDbContext.Composites.First(x => x.Name == "window");
        var boat = _testDbContext.Composites.First(x => x.Name == "boat");

        var (glass, metal) = GetGlassAndMetal(_testDbContext);
        var purchases = GetTestElementPurchases(glass, metal);

        var compositeSoldEvents = GetTestCompositeSoldEvents(window, boat);

        _testDbContext.ElementPurchasedEvents.AddRange(purchases);
        _testDbContext.CompositeSoldEvents.AddRange(compositeSoldEvents);
        await _testDbContext.BaseSaveChangesAsync();

        // assert
        var hResult = await _handler.Handle(new Unit());

        Assert.Equal(HandlerResultStatus.Ok, hResult.Status);

        var glassStats = hResult.Object!.First(x => x.ElementName == "glass");
        var metalStats = hResult.Object!.First(x => x.ElementName == "metal");

        AssertStats(glassStats, unitsUsedInComposites: 7, unitsInStock: 13, unitsPurchased: 20, purchaseCosts: 20);
        AssertStats(metalStats, unitsUsedInComposites: 10, unitsInStock: 10, unitsPurchased: 20, purchaseCosts: 40);
    }

    private static void AssertStats(
        ElementStatisticsDto details,
        double unitsUsedInComposites,
        double unitsInStock,
        int unitsPurchased,
        double purchaseCosts)
    {
        Assert.Equal(unitsUsedInComposites, details.UnitsUsedInComposites);
        Assert.Equal(unitsInStock, details.UnitsInStock);
        Assert.Equal(unitsPurchased, details.UnitsPurchased);
        Assert.Equal(purchaseCosts, details.PurchaseCosts);
    }

    private static (Element glass, Element metal) GetGlassAndMetal(IAppDbContext testDbContext)
    {
        var drinks = testDbContext.Elements
            .Where(x => x.Name == "glass" || x.Name == "metal")
            .Include(x => x.UnitOfMeasure)
            .ToList();

        var glass = drinks.First(x => x.Name == "glass");
        var metal = drinks.First(x => x.Name == "metal");

        return (glass, metal);
    }

    private static List<ElementPurchasedEvent> GetTestElementPurchases(Element glass, Element metal)
    {
        return new()
        {
            new()
            {
                ElementId = glass.Id,
                UnitOfMeasure = glass.UnitOfMeasure,
                UnitPrice = 1,
                UnitsBought = 10
            },
            new()
            {
                ElementId = glass.Id,
                UnitOfMeasure = glass.UnitOfMeasure,
                UnitPrice = 1,
                UnitsBought = 10
            },
            new()
            {
                ElementId = metal.Id,
                UnitOfMeasure = metal.UnitOfMeasure,
                UnitPrice = 2,
                UnitsBought = 20
            }
        };
    }
    private static List<CompositeSoldEvent> GetTestCompositeSoldEvents(Composite window, Composite boat)
    {
        return new()
        {
            new()
            {
                CompositeId = window.Id,
                UnitPrice = 5,
                UnitsSold = 1
            },
            new()
            {
                CompositeId = window.Id,
                UnitPrice = 5,
                UnitsSold = 1
            },
            new()
            {
                CompositeId = boat.Id,
                UnitPrice = 10,
                UnitsSold = 1,
                TotalDiscountPercent = 10
            }
        };
    }
}
