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
        _testDbContext = new ContextCreator(CarpenterDbMocker.GetSeed).GetTestDbContext();
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


        var (glassStatsExpected, metalStatsExpected) =
            GetExpectedValues(new List<(Element E, double UnitsUsedInComposites, int UsedInComposites)>
            {
                (glass, 0d, 2),
                (metal, 0d, 1)
            }, new List<ElementSoldEvent>());

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

        var (glassStatsExpected, metalStatsExpected) =
            GetExpectedValues(new List<(Element E, double UnitsUsedInComposites, int UsedInComposites)>
            {
                (glass, 0d, 2),
                (metal, 0d, 1)
            }, sales);

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

        var (glassStatsExpected, metalStatsExpected) =
            GetExpectedValues(new List<(Element E, double UnitsUsedInComposites, int UsedInComposites)>
            {
                (glass, 12d, 2),
                (metal, 20d, 1)
            }, new List<ElementSoldEvent>());

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

        var (glassStatsExpected, metalStatsExpected) =
            GetExpectedValues(new List<(Element E, double UnitsUsedInComposites, int UsedInComposites)>
            {
                (glass, 12d, 2),
                (metal, 20d, 1)
            }, sales);

        Assert.Equivalent(glassStatsExpected, glassStats);
        Assert.Equivalent(metalStatsExpected, metalStats);
    }

    private static (ElementStatisticsDto glassExpected, ElementStatisticsDto metalExpected)
        GetExpectedValues(List<(Element E, double UnitsUsedInComposites, int UsedInComposites)> elementData, List<ElementSoldEvent> soldEvents)
    {
        var glass = elementData.Single(x => x.E.Name == "glass");
        var metal = elementData.Single(x => x.E.Name == "metal");
        var purchaseEvents = GetTestElementPurchases(glass.E, metal.E);

        double SafeAverage<T>(List<T> xs, Func<T, double> selector) =>
            xs.Count > 0 ? xs.Average(selector) : 0d;

        ElementStatisticsDto Compute(Element el, double unitsUsedInComposites, int usedInComposites)
        {
            var xp = purchaseEvents.Where(e => e.ElementId == el.Id).ToList();
            var xs = soldEvents.Where(e => e.ElementId == el.Id).ToList();

            return new ElementStatisticsDto
            {
                ElementId = el.Id,
                Name = el.Name,
                Unit = el.UnitOfMeasure!.Name,
                UnitsUsedInComposites = unitsUsedInComposites,
                UsedInComposites = usedInComposites,
                UnitsInStock = xp.Sum(p => p.UnitsBought) - xs.Sum(s => s.UnitsSold) - unitsUsedInComposites,
                UnitsPurchased = xp.Sum(p => p.UnitsBought),
                PurchaseCosts = xp.Sum(p => Resourcerer.Utilities.Maths.Discount(p.UnitsBought * p.UnitPrice, p.TotalDiscountPercent)),
                AveragePurchaseDiscount = SafeAverage(xp, p => p.TotalDiscountPercent),
                UnitsSold = xs.Sum(s => s.UnitsSold),
                SalesEarning = xs.Sum(s => Resourcerer.Utilities.Maths.Discount(s.UnitsSold * s.UnitPrice, s.TotalDiscountPercent)),
                AverageSaleDiscount = SafeAverage(xs, s => s.TotalDiscountPercent)
            };
        }

        var glassExpected = Compute(glass.E, glass.UnitsUsedInComposites, glass.UsedInComposites);
        var metalExpected = Compute(metal.E, metal.UnitsUsedInComposites, metal.UsedInComposites);
        
        return (glassExpected, metalExpected);
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
