using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.Elements;
using Resourcerer.Logic;
using Resourcerer.Logic.Queries.Elements;
using Resourcerer.UnitTests.Utilities;

namespace Resourcerer.UnitTests.Logic.HandlerTests.Elements.Queries;

public class GetAllElementsStatisticsTests
{
    private readonly IAppDbContext _testDbContext;
    private readonly GetAllElementsStatistics.Handler _handler;

    public GetAllElementsStatisticsTests()
    {
        _testDbContext = new ContextCreator(seedEvents: false).GetTestDbContext();
        _handler = new GetAllElementsStatistics.Handler(_testDbContext);
    }

    [Fact]
    public async Task CorrectlySums_UsageDetails_When_ElementsArePurchased()
    {
        // arrange
        var (rum, gin, lime) = GetRumGinAndLime(_testDbContext);
        var purchases = GetTestElementPurchases(rum, gin, lime);

        _testDbContext.ElementPurchasedEvents.AddRange(purchases);
        await _testDbContext.BaseSaveChangesAsync();

        // assert
        var hResult = await _handler.Handle(new Unit());

        Assert.Equal(HandlerResultStatus.Ok, hResult.Status);

        var rumStats = hResult.Object!.First(x => x.ElementName == "rum");
        var ginStats = hResult.Object!.First(x => x.ElementName == "gin");
        var limeStats = hResult.Object!.First(x => x.ElementName == "lime");

        AssertUsageDetails(rumStats, unitsUsedInComposites: 0, unitsInStock: 3, unitsPurchased: 3, purchaseCosts: 25);
        AssertUsageDetails(ginStats, unitsUsedInComposites: 0, unitsInStock: 1, unitsPurchased: 1, purchaseCosts: 5);
        AssertUsageDetails(limeStats, unitsUsedInComposites: 0, unitsInStock: 1, unitsPurchased: 1, purchaseCosts: 10);
    }

    [Fact]
    public async void CorrectlySumsUsageDetails_When_ElementIsSold()
    {
        // arrange
        var (rum, gin, lime) = GetRumGinAndLime(_testDbContext);
        var purchases = GetTestElementPurchases(rum, gin, lime);
        var sales = purchases.Select(x => new ElementSoldEvent
        {
            ElementId = x.ElementId,
            PriceByUnit = x.PriceByUnit,
            UnitsSold = x.UnitsBought,
            UnitOfMeasure = x.UnitOfMeasure
        });

        _testDbContext.ElementPurchasedEvents.AddRange(purchases);
        _testDbContext.ElementSoldEvents.AddRange(sales);
        await _testDbContext.BaseSaveChangesAsync();

        // assert
        var hResult = await _handler.Handle(new Unit());

        Assert.Equal(HandlerResultStatus.Ok, hResult.Status);

        var rumStats = hResult.Object!.First(x => x.ElementName == "rum");
        var ginStats = hResult.Object!.First(x => x.ElementName == "gin");
        var limeStats = hResult.Object!.First(x => x.ElementName == "lime");

        AssertUsageDetails(rumStats, unitsUsedInComposites: 0, unitsInStock: 0, unitsPurchased: 3, purchaseCosts: 25);
        AssertUsageDetails(ginStats, unitsUsedInComposites: 0, unitsInStock: 0, unitsPurchased: 1, purchaseCosts: 5);
        AssertUsageDetails(limeStats, unitsUsedInComposites: 0, unitsInStock: 0, unitsPurchased: 1, purchaseCosts: 10);
    }

    [Fact]
    public async void CorrectlySumsUsageDetails_When_CompositeIsSold()
    {
        // arrange
        var darkNstormy = _testDbContext
            .Composites
            .First(x => x.Name == "dark n stormy");
        
        var ginFizz = _testDbContext
            .Composites
            .First(x => x.Name == "gin fizz");

        var (rum, gin, lime) = GetRumGinAndLime(_testDbContext);
        var purchases = GetTestElementPurchases(rum, gin, lime);
        var compositeSoldEvents = GetTestCompositeSoldEvents(darkNstormy, ginFizz);

        _testDbContext.ElementPurchasedEvents.AddRange(purchases);
        _testDbContext.CompositeSoldEvents.AddRange(compositeSoldEvents);
        await _testDbContext.BaseSaveChangesAsync();

        // assert
        var hResult = await _handler.Handle(new Unit());

        Assert.Equal(HandlerResultStatus.Ok, hResult.Status);

        var rumStats = hResult.Object!.First(x => x.ElementName == "rum");
        var ginStats = hResult.Object!.First(x => x.ElementName == "gin");
        var limeStats = hResult.Object!.First(x => x.ElementName == "lime");

        AssertUsageDetails(rumStats, unitsUsedInComposites: 0.015, unitsInStock: 2.985, unitsPurchased: 3, purchaseCosts: 25);
        AssertUsageDetails(ginStats, unitsUsedInComposites: 0.005, unitsInStock: 0.995, unitsPurchased: 1, purchaseCosts: 5);
        AssertUsageDetails(limeStats, unitsUsedInComposites: 0.1, unitsInStock: 0.9, unitsPurchased: 1, purchaseCosts: 10);
    }

    private static void AssertUsageDetails(
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

    private static (Element rum, Element gin, Element lime) GetRumGinAndLime(IAppDbContext testDbContext)
    {
        var drinks = testDbContext.Elements
            .Where(x => x.Name == "rum" || x.Name == "gin" || x.Name == "lime")
            .Include(x => x.UnitOfMeasure)
            .ToList();

        var rum = drinks.First(x => x.Name == "rum");
        var gin = drinks.First(x => x.Name == "gin");
        var lime = drinks.First(x => x.Name == "lime");

        return (rum, gin, lime);
    }
    private static List<ElementPurchasedEvent> GetTestElementPurchases(Element rum, Element gin, Element lime)
    {
        return new List<ElementPurchasedEvent>
        {
            new ElementPurchasedEvent
            {
                ElementId = rum.Id,
                PriceByUnit = 10,
                UnitsBought = 2,
                UnitOfMeasure = rum.UnitOfMeasure
            },
            new ElementPurchasedEvent
            {
                ElementId = rum.Id,
                PriceByUnit = 5,
                UnitsBought = 1,
                UnitOfMeasure = rum.UnitOfMeasure
            },
            new ElementPurchasedEvent
            {
                ElementId = gin.Id,
                PriceByUnit = 5,
                UnitsBought = 1,
                UnitOfMeasure = gin.UnitOfMeasure
            },
            new ElementPurchasedEvent
            {
                ElementId = lime.Id,
                PriceByUnit = 10,
                UnitsBought = 1,
                UnitOfMeasure = lime.UnitOfMeasure
            }
        };
    }
    private static List<CompositeSoldEvent> GetTestCompositeSoldEvents(Composite darkNstormy, Composite ginFizz)
    {
        return new List<CompositeSoldEvent>
        {
            new()
            {
                CompositeId = darkNstormy.Id,
                PriceByUnit = 1,
                UnitsSold = 1
            },
            new()
            {
                CompositeId = darkNstormy.Id,
                PriceByUnit = 1,
                UnitsSold = 2
            },
            new()
            {
                CompositeId = ginFizz.Id,
                PriceByUnit = 1,
                UnitsSold = 1
            }
        };
    }
    
}
