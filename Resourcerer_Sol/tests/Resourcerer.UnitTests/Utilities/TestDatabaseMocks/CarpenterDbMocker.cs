using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Mocks;
using Resourcerer.Logic.Queries.Mocks;

namespace Resourcerer.UnitTests.Utilities.TestDatabaseMocks;

public class CarpenterDbMocker : DbMockingFuncs
{
    public static DatabaseData GetSeed()
    {
        // users
        var admin = MakeUser("admin", "admin", true);

        // categories
        var material = MakeCategory("material");
        var product = MakeCategory("product");

        // units of measure
        var uom = MakeUnitOfMeasure("unit", "u");

        // elements
        var glass = MakeElement("glass", material, uom);
        var metal = MakeElement("metal", material, uom);

        // composites
        var window = MakeComposite("window", product);
        var boat = MakeComposite("boat", product);

        // prices
        var glassPrice = MakePrice(5, glass);
        var metalPrice = MakePrice(10, metal);
        var windowPrice = MakePrice(10, window);
        var boatPrice = MakePrice(150, boat);

        var glassPriceOld = MakePrice(7, glass);
        var metalPriceOld = MakePrice(12, metal);
        var windowPriceOld = MakePrice(12, window);
        var boatPriceOld = MakePrice(152, boat);

        MarkDeleted(glassPriceOld, metalPriceOld, windowPriceOld, boatPriceOld);

        var excerptData = new List<(Composite, List<(Element, double)>)>
        {
            (window, new List<(Element, double)>()
            {
                (glass, 1)
            }),
            (boat, new List<(Element, double)>()
            {
                (metal, 10), (glass, 5)
            })
        };

        var excerpts = excerptData
            .Select(x => MakeExcerpts(x.Item1, x.Item2))
            .SelectMany(x => x);

        return new DatabaseData
        {
            AppUsers = new[] { admin },
            Categories = new[] { material, product },
            Excerpts = excerpts,
            UnitsOfMeasure = new[] { uom },
            Prices = new[] { glassPrice, metalPrice, windowPrice, boatPrice, glassPriceOld, metalPriceOld, windowPriceOld, boatPriceOld },
            Composites = new[] { window, boat },
            CompositeSoldEvents = Array.Empty<CompositeSoldEvent>(),
            Elements = new[] { glass, metal },
            ElementSoldEvents = Array.Empty<ElementInstanceSoldEvent>(),
            ElementPurchasedEvents = Array.Empty<ElementPurchasedEvent>()
        };
    }
}
