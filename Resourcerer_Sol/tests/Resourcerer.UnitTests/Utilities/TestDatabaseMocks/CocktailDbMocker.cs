using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Logic.Queries.Mocks;
using System.Diagnostics.Metrics;

namespace Resourcerer.UnitTests.Utilities.TestDatabaseMocks;

public class CocktailDbMocker : DbMockingFuncs
{
    public static async Task SeedAsync(IAppDbContext context)
    {
        // users
        var adminUser = MakeUser("admin", "admin", true);
        var loserUser = MakeUser("user", "user", false);

        // categories
        var bar = MakeCategory("Bar");
        var spirits = MakeCategory("Spirits", bar);
        var ales = MakeCategory("Ales", bar);
        var waters = MakeCategory("Waters", bar);
        var veggies = MakeCategory("Veggies", bar);
        var cocktails = MakeCategory("Cocktails", bar);
        var tikiCocktails = MakeCategory("Tiki", cocktails);

        // units of measure
        var liter = MakeUnitOfMeasure("Liter", "l");
        var kg = MakeUnitOfMeasure("Kilogram", "kg");

        // elements
        var vodka = MakeElement("vodka", spirits, liter);
        var rum = MakeElement("rum", spirits, liter);
        var gin = MakeElement("gin", spirits, liter);
        var gingerAle = MakeElement("ginger ale", ales, liter);
        var sparklingWater = MakeElement("sparkling water", waters, liter);
        var lime = MakeElement("lime", veggies, kg);

        // composites
        var moscowMule = MakeComposite("moscow mule", cocktails);
        var darkNstormy = MakeComposite("dark n stormy", cocktails);
        var ginFizz = MakeComposite("gin fizz", cocktails);

        // prices
        var p1 = MakePrice(6, moscowMule);
        var p2 = MakePrice(7, darkNstormy);
        var p3 = MakePrice(4, ginFizz);
        var p4 = MakePrice(2, sparklingWater);

        // excerpts
        var excerptData = new List<(Composite, List<(Element, double)>)>
        {
                (moscowMule, new List<(Element, double)>()
                {
                    (vodka, 0.005d), (gingerAle, 0.003d), (lime, 0.025d)
                }),
                (darkNstormy, new List<(Element, double)>()
                {
                    (rum, 0.005d), (gingerAle, 0.003d), (lime, 0.025d)
                }),
                (ginFizz, new List<(Element, double)>()
                {
                    (gin, 0.005d), (sparklingWater, 0.003d), (lime, 0.025d)
                })
        };

        var excerpts = excerptData
            .Select(x => MakeExcerpts(x.Item1, x.Item2))
            .SelectMany(x => x);

        context.AppUsers.AddRange(adminUser, loserUser);
        context.Categories.AddRange(bar, spirits, ales, waters, veggies, cocktails, tikiCocktails);
        context.Excerpts.AddRange(excerpts);
        context.UnitsOfMeasure.AddRange(liter, kg);
        context.Prices.AddRange(p1, p2, p3, p4);
        context.Composites.AddRange(moscowMule, darkNstormy, ginFizz);
        context.CompositeSoldEvents.AddRange();
        context.Elements.AddRange(vodka, rum, gin, gingerAle, sparklingWater, lime);
        context.ElementSoldEvents.AddRange();
        context.ElementPurchasedEvents.AddRange();

        await context.SaveChangesAsync();
    }
}
