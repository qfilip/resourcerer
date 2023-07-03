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
    public static List<ElementSoldEvent> GetTestElementSoldEvents(Element glass, Element metal)
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
}
