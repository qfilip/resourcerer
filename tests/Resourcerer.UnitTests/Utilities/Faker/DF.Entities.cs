using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Entities.JsonEntities;
using Resourcerer.DataAccess.Enums;
using Resourcerer.Dtos;
using Resourcerer.Utilities;
using Resourcerer.Utilities.Cryptography;
using System.Text.Json;

namespace Resourcerer.UnitTests.Utilities.Faker;

internal static partial class DF
{
    public static DateTime Now = new DateTime(2000, 1, 1);
    public static string MakeName() => $"test-{Guid.NewGuid().ToString("n").Substring(0, 6)}";
    public static T MakeEntity<T>(Func<T> retn) where T : AppDbEntity
    {
        var e = retn();
        if(e is AppDbJsonField jeb)
        {
            jeb.Id = jeb.Id ?? MiniId.Generate();
        }
        else
        {
            e.Id = e.Id != Guid.Empty ? e.Id : Guid.NewGuid();
        }
        e.CreatedAt = Now;
        e.ModifiedAt = Now;

        return e;
    }

    public static Company FakeCompany(AppDbContext context, Action<Company>? modifier = null)
    {
        var entity = MakeEntity(() => new Company { Name = MakeName() });
        modifier?.Invoke(entity);
        context.Companies.Add(entity);

        return entity;
    }

    public static AppUser FakeUser(AppDbContext context, string password, Action<AppUser>? modifier = null, bool setAdminPermissions = false)
    {
        var company = FakeCompany(context);
        var user = MakeEntity(() => new AppUser
        {
            Name = MakeName(),
            PasswordHash = Hasher.GetSha256Hash(password),
            
            CompanyId = company.Id,
            Company = company
        });

        if(setAdminPermissions)
        {
            var permissionDict = Permissions.GetAllPermissionsDictionary();
            var permissions = JsonSerializer.Serialize(permissionDict);
            user.Permissions = permissions;
        }

        modifier?.Invoke(user);

        context.AppUsers.Add(user);

        return user;
    }
    public static Category FakeCategory(AppDbContext context, Action<Category>? modifier = null)
    {
        var id = Guid.NewGuid();
        var company = FakeCompany(context);
        var category = MakeEntity(() => new Category
        {
            Name = MakeName(),
            
            CompanyId = company.Id,
            Company = company
        });

        modifier?.Invoke(category);

        context.Categories.Add(category);

        return category;
    }

    public static Item FakeItem(AppDbContext context, Action<Item>? modifier = null, double unitValue = 1, int priceCount = 3, bool pricesCorrupted = false)
    {
        var category = FakeCategory(context);
        var uom = FakeUnitOfMeasure(context);
        var entity = MakeEntity(() => new Item
        {
            Name = MakeName(),
            ExpirationTimeSeconds = 1200,
            ProductionTimeSeconds = 10,
            
            CategoryId = category.Id,
            Category = category,

            UnitOfMeasureId = uom.Id,
            UnitOfMeasure = uom
        });

        modifier?.Invoke(entity);

        context.Items.Add(entity);

        return entity;
    }

    public static Instance FakeInstance(AppDbContext context, Action<Instance>? modifier = null)
    {
        var company = FakeCompany(context);
        var item = FakeItem(context);
        var entity = MakeEntity(() => new Instance
        {
            Quantity = 1,

            ItemId = item.Id,
            Item = item,
            
            OwnerCompanyId = company.Id,
            OwnerCompany = company
        });

        modifier?.Invoke(entity);

        context.Instances.Add(entity);

        return entity;
    }

    public static ItemProductionOrder FakeItemProductionOrder(AppDbContext context, Action<ItemProductionOrder> modifier)
    {
        var item = DF.FakeItem(context);
        var entity = MakeEntity(() => new ItemProductionOrder
        {
            Item = item,
            ItemId = item.Id
        });

        modifier.Invoke(entity);

        context.ItemProductionOrders.Add(entity);

        return entity;
    }

    public static UnitOfMeasure FakeUnitOfMeasure(AppDbContext context, Action<UnitOfMeasure>? modifier = null)
    {
        var company = FakeCompany(context);
        var uom = MakeEntity(() => new UnitOfMeasure
        {
            Name = MakeName(),
            Symbol = "test",

            CompanyId = company.Id,
            Company = company
        });

        modifier?.Invoke(uom);

        context.UnitsOfMeasure.Add(uom);

        return uom;
    }

    public static List<Excerpt> FakeExcerpts(AppDbContext context, Item composite, (Item, double)[] itemsDetail)
    {
        var excerpts = new List<Excerpt>();
        foreach (var d in itemsDetail)
        {
            excerpts.Add(MakeEntity(() => new Excerpt
            {
                CompositeId = composite.Id,
                ElementId = d.Item1.Id,
                Quantity = d.Item2
            }));
        }

        context.Excerpts.AddRange(excerpts);

        return excerpts;
    }

    public static Price FakePrice(AppDbContext context, Action<Price>? modifier = null)
    {
        var entity = MakeEntity(() => new Price
        {
            UnitValue = 1,
            ItemId = FakeItem(context).Id
        });

        modifier?.Invoke(entity);

        return entity;
    }

    public static List<Price> FakePrices(Action<Price>? modifier, double unitValue, int priceCount, bool pricesCorrupted)
    {
        if (priceCount < 0)
        {
            throw new ArgumentException($"priceCount cannot be a negative number");
        }

        var prices = Enumerable.Range(0, priceCount)
           .Select(x => MakeEntity(() => new Price
           {
               Id = Guid.NewGuid(),
               UnitValue = unitValue,
               EntityStatus = x == 0 ? eEntityStatus.Active : eEntityStatus.Deleted
           })).ToList();

        prices.ForEach(x => modifier?.Invoke(x));

        if (pricesCorrupted)
            prices.ForEach(x => x.EntityStatus = eEntityStatus.Active);

        return prices;
    }
}
