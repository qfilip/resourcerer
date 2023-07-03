using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Enums;
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

        var glassStats = hResult.Object!.First(x => x.Name == "glass");
        var metalStats = hResult.Object!.First(x => x.Name == "metal");
        
        var glassStatsExpected = new ElementStatisticsDto
        {
            ElementId = glass.Id,
            Name = glass.Name,
            Unit = glass.UnitOfMeasure!.Name,
            UnitsUsedInComposites = 0,
            UnitsInStock = 25,
            UnitsPurchased = 25,
            PurchaseCosts = 24,
            UnitsSold = 0,
            SalesEarning = 0
        };
        var metalStatsExpected = new ElementStatisticsDto
        {
            ElementId = metal.Id,
            Name = metal.Name,
            Unit = metal.UnitOfMeasure!.Name,
            UnitsUsedInComposites = 0,
            UnitsInStock = 25,
            UnitsPurchased = 25,
            PurchaseCosts = 50,
            UnitsSold = 0,
            SalesEarning = 0
        };

        Assert.Equivalent(glassStatsExpected, glassStats);
        Assert.Equivalent(metalStatsExpected, metalStats);        
    }

    [Fact]
    public async void CorrectlySumsUsageDetails_When_ElementIsSold()
    {
        // arrange
        var (glass, metal) = GetGlassAndMetal(_testDbContext);
        
        var purchases = GetTestElementPurchases(glass, metal);
        var sales = GetTestElementSoldEvents(glass, metal);

        _testDbContext.ElementPurchasedEvents.AddRange(purchases);
        _testDbContext.ElementSoldEvents.AddRange(sales);
        
        await _testDbContext.BaseSaveChangesAsync();

        // assert
        var hResult = await _handler.Handle(new Unit());

        Assert.Equal(HandlerResultStatus.Ok, hResult.Status);

        var glassStats = hResult.Object!.First(x => x.Name == "glass");
        var metalStats = hResult.Object!.First(x => x.Name == "metal");

        var glassStatsExpected = new ElementStatisticsDto
        {
            ElementId = glass.Id,
            Name = glass.Name,
            Unit = glass.UnitOfMeasure!.Name,
            UnitsUsedInComposites = 0,
            UnitsInStock = 15,
            UnitsPurchased = 25,
            PurchaseCosts = 24,
            UnitsSold = 10,
            SalesEarning = 50
        };
        var metalStatsExpected = new ElementStatisticsDto
        {
            ElementId = metal.Id,
            Name = metal.Name,
            Unit = metal.UnitOfMeasure!.Name,
            UnitsUsedInComposites = 0,
            UnitsInStock = 20,
            UnitsPurchased = 25,
            PurchaseCosts = 50,
            UnitsSold = 5,
            SalesEarning = 40
        };

        Assert.Equivalent(glassStatsExpected, glassStats);
        Assert.Equivalent(metalStatsExpected, metalStats);
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

        var glassStats = hResult.Object!.First(x => x.Name == "glass");
        var metalStats = hResult.Object!.First(x => x.Name == "metal");

        var glassStatsExpected = new ElementStatisticsDto
        {
            ElementId = glass.Id,
            Name = glass.Name,
            Unit = glass.UnitOfMeasure!.Name,
            UnitsUsedInComposites = 12,
            UnitsInStock = 13,
            UnitsPurchased = 25,
            PurchaseCosts = 24,
            UnitsSold = 0,
            SalesEarning = 0
        };
        var metalStatsExpected = new ElementStatisticsDto
        {
            ElementId = metal.Id,
            Name = metal.Name,
            Unit = glass.UnitOfMeasure!.Name,
            UnitsUsedInComposites = 20,
            UnitsInStock = 5,
            UnitsPurchased = 25,
            PurchaseCosts = 50,
            UnitsSold = 0,
            SalesEarning = 0
        };

        Assert.Equivalent(glassStatsExpected, glassStats);
        Assert.Equivalent(metalStatsExpected, metalStats);
    }

    [Fact]
    public async void CorrectlySumsUsageDetails_When_CompositeIsSold_And_ElementIsSold()
    {
        // arrange
        var window = _testDbContext.Composites.First(x => x.Name == "window");
        var boat = _testDbContext.Composites.First(x => x.Name == "boat");

        var (glass, metal) = GetGlassAndMetal(_testDbContext);
        
        var purchases = GetTestElementPurchases(glass, metal);
        var sales = GetTestElementSoldEvents(glass, metal);
        var compositeSoldEvents = GetTestCompositeSoldEvents(window, boat);

        _testDbContext.ElementPurchasedEvents.AddRange(purchases);
        _testDbContext.ElementSoldEvents.AddRange(sales);
        _testDbContext.CompositeSoldEvents.AddRange(compositeSoldEvents);

        await _testDbContext.BaseSaveChangesAsync();

        // assert
        var hResult = await _handler.Handle(new Unit());

        Assert.Equal(HandlerResultStatus.Ok, hResult.Status);

        var glassStats = hResult.Object!.First(x => x.Name == "glass");
        var metalStats = hResult.Object!.First(x => x.Name == "metal");

        var glassStatsExpected = new ElementStatisticsDto
        {
            ElementId = glass.Id,
            Name = glass.Name,
            Unit = glass.UnitOfMeasure!.Name,
            UnitsUsedInComposites = 12,
            UnitsInStock = 3,
            UnitsPurchased = 25,
            PurchaseCosts = 24,
            UnitsSold = 10,
            SalesEarning = 50
        };
        var metalStatsExpected = new ElementStatisticsDto
        {
            ElementId = metal.Id,
            Name = metal.Name,
            Unit = glass.UnitOfMeasure!.Name,
            UnitsUsedInComposites = 20,
            UnitsInStock = 0,
            UnitsPurchased = 25,
            PurchaseCosts = 50,
            UnitsSold = 5,
            SalesEarning = 40
        };

        Assert.Equivalent(glassStatsExpected, glassStats);
        Assert.Equivalent(metalStatsExpected, metalStats);
    }

    private static (Element glass, Element metal) GetGlassAndMetal(IAppDbContext testDbContext)
    {
        var drinks = testDbContext.Elements
            .Where(x => x.Name == "glass" || x.Name == "metal")
            .Include(x => x.UnitOfMeasure)
            .Include(x => x.Prices)
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
                UnitsBought = 15
            },
            new()
            {
                ElementId = glass.Id,
                UnitOfMeasure = glass.UnitOfMeasure,
                UnitPrice = 1,
                UnitsBought = 10,
                TotalDiscountPercent = 10
            },
            new()
            {
                ElementId = metal.Id,
                UnitOfMeasure = metal.UnitOfMeasure,
                UnitPrice = 2,
                UnitsBought = 25
            }
        };
    }
    private static List<ElementSoldEvent> GetTestElementSoldEvents(Element glass, Element metal)
    {
        return new()
        {
            new()
            {
                ElementId = glass.Id,
                UnitOfMeasure = glass.UnitOfMeasure,
                UnitPrice = glass.Prices.Single(x => x.EntityStatus == eEntityStatus.Active).UnitValue,
                UnitsSold = 5
            },
            new()
            {
                ElementId = glass.Id,
                UnitOfMeasure = glass.UnitOfMeasure,
                UnitPrice = glass.Prices.Single(x => x.EntityStatus == eEntityStatus.Active).UnitValue,
                UnitsSold = 5
            },
            new()
            {
                ElementId = metal.Id,
                UnitOfMeasure = metal.UnitOfMeasure,
                UnitPrice = metal.Prices.Single(x => x.EntityStatus == eEntityStatus.Active).UnitValue,
                UnitsSold = 5,
                TotalDiscountPercent = 20
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
                UnitsSold = 2,
                TotalDiscountPercent = 10
            }
        };
    }
}
