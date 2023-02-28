using MediatR;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;

namespace Resourcerer.Logic.Mocks.Commands;
public static class SeedMockData
{
    private static DateTime now = DateTime.Now;
    public class Command : IRequest<Unit>
    {
    }

    public class Handler : IRequestHandler<Command, Unit>
    {
        private readonly AppDbContext _dbContext;

        public Handler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
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

            var prices = new Price[] { p1, p2, p3 };

            var pur1 = MakeElementPurchasedEvent(vodka, 1, 10);
            var pur2 = MakeElementPurchasedEvent(rum, 1, 20);
            var pur3 = MakeElementPurchasedEvent(gin, 1, 5);
            var pur4 = MakeElementPurchasedEvent(gingerAle, 1, 15);
            var pur5 = MakeElementPurchasedEvent(sparklingWater, 1, 2);
            var pur6 = MakeElementPurchasedEvent(lime, 0.5d, 5);

            var purchases = new ElementPurchasedEvent[] { pur1, pur2, pur3, pur4, pur5, pur6 };

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

            _dbContext.Categories.AddRange(categories);
            _dbContext.UnitsOfMeasure.AddRange(uoms);
            _dbContext.Elements.AddRange(elements);
            _dbContext.Composites.AddRange(composites);
            _dbContext.Excerpts.AddRange(excerpts);
            _dbContext.Prices.AddRange(prices);
            _dbContext.ElementPurchasedEvents.AddRange(purchases);

            await _dbContext.SaveChangesAsync();

            return new Unit();
        }
    }

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

    private static Price MakePrice(int value, Composite composite)
    {
        return MakeEntity(() => new Price
        {
            Value = value,
            CompositeId = composite.Id
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

