using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Mocks;

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

            var cp1 = MakePrice(6, moscowMule);
            var cp2 = MakePrice(7, darkNstormy);
            var cp3 = MakePrice(4, ginFizz);

            var compositePrices = new CompositePrice[] { cp1, cp2, cp3 };

            var pur1 = MakeElementPurchasedEvent(vodka, 1, 10);
            var pur2 = MakeElementPurchasedEvent(vodka, 1, 10);
            var pur3 = MakeElementPurchasedEvent(rum, 1, 20);
            var pur4 = MakeElementPurchasedEvent(gin, 1, 5);
            var pur5 = MakeElementPurchasedEvent(gingerAle, 1, 15);
            var pur6 = MakeElementPurchasedEvent(sparklingWater, 1, 2);
            var pur7 = MakeElementPurchasedEvent(lime, 0.5d, 5);

            var purchases = new ElementPurchasedEvent[] { pur1, pur2, pur3, pur4, pur5, pur6, pur7 };

            var ep = MakePrice(2, sparklingWater);
            var elementPrices = new ElementPrice[] { ep };

            var rese = MakeElementSoldEvent(sparklingWater, ep);

            var elementSoldEvents = new ElementSoldEvent[] { rese };
            
            var cse1 = MakeCompositeSoldEvent(moscowMule, cp1);
            var cse2 = MakeCompositeSoldEvent(moscowMule, cp1);
            var cse3 = MakeCompositeSoldEvent(darkNstormy, cp2);

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
                Composites = composites,
                CompositePrices = compositePrices,
                CompositeSoldEvents = sales,

                Elements = elements,
                ElementPrices = elementPrices,
                ElementSoldEvents = elementSoldEvents,
                ElementPurchasedEvents = purchases,
                
                Excerpts = excerpts,
                UnitOfMeasures = uoms
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

    private static CompositePrice MakePrice(int value, Composite composite)
    {
        return MakeEntity(() => new CompositePrice
        {
            Value = value,
            CompositeId = composite.Id
        });
    }

    private static ElementPrice MakePrice(int value, Element element)
    {
        return MakeEntity(() => new ElementPrice
        {
            Value = value,
            ElementId = element.Id
        });
    }

    private static ElementPurchasedEvent MakeElementPurchasedEvent(Element element, double unitPrice, int numOfUnits)
    {
        return MakeEntity(() => new ElementPurchasedEvent
        {
            ElementId = element.Id,
            NumOfUnits = numOfUnits,
            UnitPrice = unitPrice
        });
    }

    private static ElementSoldEvent MakeElementSoldEvent(Element element, ElementPrice price)
    {
        return MakeEntity(() => new ElementSoldEvent
        {
            ElementId = element.Id,
            PriceId = price.Id
        });
    }

    private static CompositeSoldEvent MakeCompositeSoldEvent(Composite composite, CompositePrice price)
    {
        return MakeEntity(() => new CompositeSoldEvent
        {
            CompositeId = composite.Id,
            PriceId = price.Id
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

