using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Enums;

namespace Resourcerer.UnitTests.Utilities.TestDatabaseMocks;

public class CarpenterDbEventMocker
{
    public static List<ElementPurchasedEvent> GetTestElementPurchases(Element glass, Element metal)
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
    public static List<ElementInstanceSoldEvent> GetTestElementSoldEvents(Element glass, Element metal)
    {
        return new()
        {
            new()
            {
                // ElementId = glass.Id,
                UnitOfMeasure = glass.UnitOfMeasure,
                UnitPrice = glass.Prices.Single(x => x.EntityStatus == eEntityStatus.Active).UnitValue,
                UnitsSold = 5
            },
            new()
            {
                // ElementId = glass.Id,
                UnitOfMeasure = glass.UnitOfMeasure,
                UnitPrice = glass.Prices.Single(x => x.EntityStatus == eEntityStatus.Active).UnitValue,
                UnitsSold = 5
            },
            new()
            {
                // ElementId = metal.Id,
                UnitOfMeasure = metal.UnitOfMeasure,
                UnitPrice = metal.Prices.Single(x => x.EntityStatus == eEntityStatus.Active).UnitValue,
                UnitsSold = 5,
                TotalDiscountPercent = 20
            }
        };
    }
    public static List<CompositeSoldEvent> GetTestCompositeSoldEvents(Composite window, Composite boat)
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

    public static (Element glass, Element metal) GetGlassAndMetal(IAppDbContext testDbContext)
    {
        var elements = testDbContext.Elements
            .Where(x => x.Name == "glass" || x.Name == "metal")
            .Include(x => x.UnitOfMeasure)
            .Include(x => x.Prices)
            .ToList();

        var glass = elements.First(x => x.Name == "glass");
        var metal = elements.First(x => x.Name == "metal");

        return (glass, metal);
    }
    public static (Composite window, Composite boat) GetWindowAndBoat(IAppDbContext testDbContext)
    {
        var composites = testDbContext.Composites
            .Where(x => x.Name == "window" || x.Name == "boat")
            .Include(x => x.Prices)
            .ToList();

        var window = composites.First(x => x.Name == "window");
        var boat = composites.First(x => x.Name == "boat");

        return (window, boat);
    }
}
