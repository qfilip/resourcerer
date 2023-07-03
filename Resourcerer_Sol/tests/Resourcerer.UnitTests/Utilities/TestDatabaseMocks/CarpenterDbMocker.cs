﻿using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Logic.Queries.Mocks;

namespace Resourcerer.UnitTests.Utilities.TestDatabaseMocks;

public class CarpenterDbMocker : DbMockingFuncs
{
    public static async Task SeedAsync(IAppDbContext context)
    {
        var admin = MakeUser("admin", "admin", true);

        var material = MakeCategory("material");
        var product = MakeCategory("product");

        var uom = MakeUnitOfMeasure("unit", "u");

        var glass = MakeElement("glass", material, uom);
        var metal = MakeElement("metal", material, uom);

        var window = MakeComposite("window", product);
        var boat = MakeComposite("boat", product);

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


        context.AppUsers.Add(admin);
        context.Categories.AddRange(material, product);
        context.Excerpts.AddRange(excerpts);
        context.UnitsOfMeasure.AddRange(uom);
        context.Prices.AddRange();
        context.Composites.AddRange(window, boat);
        context.CompositeSoldEvents.AddRange();
        context.Elements.AddRange(glass, metal);
        context.ElementSoldEvents.AddRange();
        context.ElementPurchasedEvents.AddRange();

        await context.SaveChangesAsync();
    }
}
