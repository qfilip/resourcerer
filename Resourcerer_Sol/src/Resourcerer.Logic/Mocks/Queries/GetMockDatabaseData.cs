using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Mocks;
using System.Xml.Linq;

namespace Resourcerer.Logic.Mocks.Queries;
public class GetMockDatabaseData
{
    public class Handler : IRequestHandler<Unit, DatabaseData>
    {
        public Task<HandlerResult<DatabaseData>> Handle(Unit _)
        {
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

            var pur1 = MakeElementPurchasedEvent(vodka, 1, 10, liter);
            var pur2 = MakeElementPurchasedEvent(vodka, 1, 10, liter);
            var pur3 = MakeElementPurchasedEvent(rum, 1, 20, liter);
            var pur4 = MakeElementPurchasedEvent(gin, 1, 5, liter);
            var pur5 = MakeElementPurchasedEvent(gingerAle, 1, 15, liter);
            var pur6 = MakeElementPurchasedEvent(sparklingWater, 1, 2, liter);
            var pur7 = MakeElementPurchasedEvent(lime, 1, 5, kg);

            var purchases = new ElementPurchasedEvent[] { pur1, pur2, pur3, pur4, pur5, pur6, pur7 };

            var rese = MakeElementSoldEvent(sparklingWater, liter, p4.Value);

            var elementSoldEvents = new ElementSoldEvent[] { rese };
            
            var cse1 = MakeCompositeSoldEvent(moscowMule, 1, p1.Value);
            var cse2 = MakeCompositeSoldEvent(moscowMule, 1, p1.Value);
            var cse3 = MakeCompositeSoldEvent(darkNstormy, 1, p1.Value);

            var sales = new CompositeSoldEvent[] { cse1, cse2, cse3 };

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
                Categories = categories,
                Excerpts = excerpts,
                UnitOfMeasures = uoms,
                Prices = prices,

                Composites = composites,
                CompositeSoldEvents = sales,

                Elements = elements,
                ElementSoldEvents = elementSoldEvents,
                ElementPurchasedEvents = purchases
            };

            return Task.FromResult(HandlerResult<DatabaseData>.Ok(dbdata));
        }
    }

    private static DateTime now = DateTime.Now;

    private static T MakeEntity<T>(Func<T> retn) where T : EntityBase
    {
        var e = retn();
        e.Id = Guid.NewGuid();
        e.CreatedAt = now;
        e.ModifiedAt = now;

        return e as T;
    }

    private static Category MakeCategory(string name, Category? parentCategory = null)
    {
        return MakeEntity(() => new Category
        {
            Name = name,
            ParentCategoryId = parentCategory?.Id
        });
    }

    private static UnitOfMeasure MakeUnitOfMeasure(string name, string symbol)
    {
        return MakeEntity(() => new UnitOfMeasure
        {
            Name = name,
            Symbol = symbol
        });
    }

    private static Element MakeElement(string name, Category category, UnitOfMeasure uom)
    {
        return MakeEntity(() => new Element
        {
            Name = name,
            CategoryId = category.Id,
            UnitOfMeasureId = uom.Id
        });
    }

    private static Composite MakeComposite(string name, Category category)
    {
        return MakeEntity(() => new Composite
        {
            Name = name,
            CategoryId = category.Id
        });
    }

    private static CompositeSoldEvent MakeCompositeSoldEvent(Composite composite, int unitsSold, double priceByUnit)
    {
        return MakeEntity(() => new CompositeSoldEvent
        {
            CompositeId = composite.Id,
            UnitsSold = unitsSold,
            PriceByUnit = priceByUnit
        });
    }

    private static Price MakePrice(int value, Composite composite)
    {
        return MakeEntity(() => new Price
        {
            Value = value,
            CompositeId = composite.Id
        });
    }

    private static Price MakePrice(int value, Element element)
    {
        return MakeEntity(() => new Price
        {
            Value = value,
            ElementId = element.Id
        });
    }

    private static ElementPurchasedEvent MakeElementPurchasedEvent(Element element, int unitsBought, double priceByUnit, UnitOfMeasure unitOfMeasure)
    {
        return MakeEntity(() => new ElementPurchasedEvent
        {
            ElementId = element.Id,
            UnitsBought = unitsBought,
            PriceByUnit = priceByUnit,
            UnitOfMeasure = unitOfMeasure
        });
    }

    private static ElementSoldEvent MakeElementSoldEvent(Element element, UnitOfMeasure unitOfMeasure, double priceByUnit)
    {
        return MakeEntity(() => new ElementSoldEvent
        {
            ElementId = element.Id,
            PriceByUnit = priceByUnit,
            UnitOfMeasure = unitOfMeasure
        });
    }

    private static CompositeSoldEvent MakeSoldEvent(Composite composite, int unitsSold, double priceByUnit)
    {
        return MakeEntity(() => new CompositeSoldEvent
        {
            CompositeId = composite.Id,
            UnitsSold = unitsSold,
            PriceByUnit = priceByUnit
        });
    }

    private static IEnumerable<Excerpt> MakeExcerpts(Composite composite, List<(Element, double)> ingredients)
    {
        return ingredients.Select(x =>
        {
            return MakeEntity(() => new Excerpt
            {
                CompositeId = composite.Id,
                ElementId = x.Item1.Id,
                Quantity = x.Item2
            });
        });
    }
}

