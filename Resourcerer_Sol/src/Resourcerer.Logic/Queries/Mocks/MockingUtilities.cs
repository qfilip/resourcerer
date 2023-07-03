using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Mocks;
using Resourcerer.Dtos;
using Resourcerer.Utilities.Cryptography;
using System.Text.Json;

namespace Resourcerer.Logic.Queries.Mocks;

public class MockingUtilities
{
    public DatabaseData GetData()
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
                        (gin, 0.005d), (sparklingWater, 0.003d), (lime, 0.025d)
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
            OldPrices = prices,

            Composites = composites,
            CompositeSoldEvents = Array.Empty<CompositeSoldEvent>(),

            Elements = elements,
            ElementSoldEvents = Array.Empty<ElementSoldEvent>(),
            ElementPurchasedEvents = Array.Empty<ElementPurchasedEvent>()
        };

        return dbdata;
    }

    protected static DateTime Now = new DateTime(2000, 1, 1);
    protected static T MakeEntity<T>(Func<T> retn) where T : EntityBase
    {
        var e = retn();
        e.Id = Guid.NewGuid();
        e.CreatedAt = Now;
        e.ModifiedAt = Now;

        return e as T;
    }

    protected static AppUser MakeUser(string name, string password, bool allPermissions, Dictionary<string, string>? permissions = null)
    {
        var userPermissions = allPermissions ?
            Permission.GetAllPermissionsDictionary() :
            (permissions ?? new Dictionary<string, string>());

        return MakeEntity(() => new AppUser
        {
            Name = name,
            PasswordHash = Hasher.GetSha256Hash(password),
            Permissions = JsonSerializer.Serialize(userPermissions)
        });
    }

    protected static Category MakeCategory(string name, Category? parentCategory = null)
    {
        return MakeEntity(() => new Category
        {
            Name = name,
            ParentCategoryId = parentCategory?.Id
        });
    }

    protected static UnitOfMeasure MakeUnitOfMeasure(string name, string symbol)
    {
        return MakeEntity(() => new UnitOfMeasure
        {
            Name = name,
            Symbol = symbol
        });
    }

    protected static Element MakeElement(string name, Category category, UnitOfMeasure uom)
    {
        return MakeEntity(() => new Element
        {
            Name = name,
            CategoryId = category.Id,
            UnitOfMeasureId = uom.Id
        });
    }

    protected static Composite MakeComposite(string name, Category category)
    {
        return MakeEntity(() => new Composite
        {
            Name = name,
            CategoryId = category.Id
        });
    }

    protected static CompositeSoldEvent MakeCompositeSoldEvent(Composite composite, double unitsSold, double priceByUnit, DateTime createdAt)
    {
        return MakeEntity(() => new CompositeSoldEvent
        {
            CompositeId = composite.Id,
            UnitsSold = unitsSold,
            UnitPrice = priceByUnit,

            CreatedAt = createdAt
        });
    }

    protected static Price MakePrice(double value, Composite composite)
    {
        return MakeEntity(() => new Price
        {
            UnitValue = value,
            CompositeId = composite.Id
        });
    }

    protected static Price MakePrice(double value, Element element)
    {
        return MakeEntity(() => new Price
        {
            UnitValue = value,
            ElementId = element.Id
        });
    }

    protected static ElementPurchasedEvent MakeElementPurchasedEvent(Element element, double unitsBought, double priceByUnit, UnitOfMeasure unitOfMeasure, DateTime createdAt)
    {
        return MakeEntity(() => new ElementPurchasedEvent
        {
            ElementId = element.Id,
            UnitsBought = unitsBought,
            UnitPrice = priceByUnit,
            UnitOfMeasure = unitOfMeasure,

            CreatedAt = createdAt
        });
    }

    protected static ElementSoldEvent MakeElementSoldEvent(Element element, UnitOfMeasure unitOfMeasure, double unitsSold, double priceByUnit, DateTime createdAt)
    {
        return MakeEntity(() => new ElementSoldEvent
        {
            ElementId = element.Id,
            UnitPrice = priceByUnit,
            UnitsSold = unitsSold,
            UnitOfMeasure = unitOfMeasure,

            CreatedAt = createdAt
        });
    }

    protected static IEnumerable<Excerpt> MakeExcerpts(Composite composite, List<(Element, double)> ingredients)
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
