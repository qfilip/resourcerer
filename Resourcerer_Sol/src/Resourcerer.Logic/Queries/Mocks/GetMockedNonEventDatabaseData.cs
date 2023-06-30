using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Mocks;

namespace Resourcerer.Logic.Queries.Mocks;

public static class GetMockedNonEventDatabaseData
{
    public class Handler : MockingUtilities, IRequestHandler<Unit, DatabaseData>
    {
        public Task<HandlerResult<DatabaseData>> Handle(Unit _)
        {
            var adminUser = MakeUser("admin", "admin", true);
            var loserUser = MakeUser("user", "user", false);

            var users = new AppUser[] { adminUser, loserUser };

            var bar = MakeCategory("Bar");
            var spirits = MakeCategory("Spirits", bar);
            var ales = MakeCategory("Ales", bar);
            var waters = MakeCategory("Waters", bar);
            var veggies = MakeCategory("Veggies", bar);
            var cocktails = MakeCategory("Cocktails", bar);
            var tikiCocktails = MakeCategory("Tiki", cocktails);

            var categories = new Category[] { bar, spirits, ales, waters, veggies, cocktails, tikiCocktails };

            var liter = MakeUnitOfMeasure("Liter", "l");
            var kg = MakeUnitOfMeasure("Kilogram", "kg");

            var uoms = new UnitOfMeasure[] { liter, kg };

            var vodka = MakeElement("vodka", spirits, liter);
            var rum = MakeElement("rum", spirits, liter);
            var gin = MakeElement("gin", spirits, liter);
            var gingerAle = MakeElement("ginger ale", ales, liter);
            var sparklingWater = MakeElement("sparkling water", waters, liter);
            var lime = MakeElement("lime", veggies, kg);

            var elements = new Element[] { vodka, rum, gin, gingerAle, sparklingWater, lime };

            var moscowMule = MakeComposite("moscow mule", cocktails);
            var darkNstormy = MakeComposite("dark n stormy", cocktails);
            var ginFizz = MakeComposite("gin fizz", cocktails);

            var composites = new Composite[] { moscowMule, darkNstormy, ginFizz };

            var p1 = MakePrice(6, moscowMule);
            var p2 = MakePrice(7, darkNstormy);
            var p3 = MakePrice(4, ginFizz);
            var p4 = MakePrice(2, sparklingWater);

            var prices = new Price[] { p1, p2, p3, p4 };

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
                        (rum, 0.005d), (sparklingWater, 0.003d), (lime, 0.025d)
                    })
            };

            var excerpts = excerptData
                .Select(x => MakeExcerpts(x.Item1, x.Item2))
                .SelectMany(x => x);

            var dbdata = new DatabaseData
            {
                AppUsers = users,
                Categories = categories,
                Excerpts = excerpts,
                UnitOfMeasures = uoms,
                Prices = prices,

                Composites = composites,
                CompositeSoldEvents = Array.Empty<CompositeSoldEvent>(),

                Elements = elements,
                ElementSoldEvents = Array.Empty<ElementSoldEvent>(),
                ElementPurchasedEvents = Array.Empty<ElementPurchasedEvent>()
            };

            return Task.FromResult(HandlerResult<DatabaseData>.Ok(dbdata));
        }
    }
}
