using Resourcerer.DataAccess.Abstractions;
using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Records;
using Resourcerer.Utilities;
using Resourcerer.Utilities.Cryptography;

namespace Resourcerer.UnitTests.Utilities;

internal static class DataFaking
{
    public static DateTime Now = new DateTime(2000, 1, 1);
    public static string MakeName() => $"test-{Guid.NewGuid().ToString("n").Substring(0, 6)}";
    public static string MakeEmail() => $"{MiniId.Generate(5)}@notmail.com";
    public static T MakeEntity<T>(Func<T> retn)
       where T : IId<Guid>, IAuditedEntity<Audit>, ISoftDeletable
    {
        var e = retn();

        e.Id = e.Id != Guid.Empty ? e.Id : Guid.NewGuid();

        e.AuditRecord = new()
        {
            CreatedAt = Now,
            ModifiedAt = Now
        };

        return e;
    }

    public static T MakeEntityWithCustomKey<T>(Func<T> retn)
        where T : IAuditedEntity<Audit>, ISoftDeletable
    {
        var e = retn();

        e.AuditRecord = new()
        {
            CreatedAt = Now,
            ModifiedAt = Now
        };

        return e;
    }

    internal static Dictionary<Type, Delegate> FakingFunctions = new()
    {
        {
            typeof(Company),
            () => MakeEntity(() => new Company { Name = MakeName() })
        },
        {
            typeof(AppUser),
            () => MakeEntity(() => new AppUser
            {
                Name = MakeName(),
                DisplayName = MakeName(),
                Email = MakeEmail(),
                PasswordHash = Hasher.GetSha256Hash(MakeName()),
                Permissions = "{}"
            })
        },
        {
            typeof(Category),
            () => MakeEntity(() => new Category { Name = MakeName() })
        },
        {
            typeof(UnitOfMeasure),
            () => MakeEntity(() => new UnitOfMeasure
            {
                Name = MakeName(),
                Symbol = "uom"
            })
        },
        {
            typeof(Price),
            () => MakeEntity(() => new Price { UnitValue = 1 })
        },
        {
            typeof(Excerpt),
            () => MakeEntityWithCustomKey(() => new Excerpt { Quantity = 1 })
        },
        {
            typeof(Item),
            () => MakeEntity(() => new Item
            {
                Name = MakeName(),
                ExpirationTimeSeconds = 1200,
                ProductionTimeSeconds = 10,
            })
        },
        {
            typeof(ItemProductionOrder),
            () => MakeEntity(() => new ItemProductionOrder { Quantity = 1 })
        },
        {
            typeof(Instance),
            () => MakeEntity(() => new Instance { Quantity = 1 })
        },
        {
            typeof(InstanceOrderedEvent),
            () => MakeEntity(()=> new InstanceOrderedEvent { Quantity = 1 })
        },
        {
            typeof(InstanceReservedEvent),
            () => MakeEntity(()=> new InstanceReservedEvent { Quantity = 1 })
        },
        {
            typeof(InstanceDiscardedEvent),
            () => MakeEntity(()=> new InstanceDiscardedEvent { Quantity = 1 })
        }
    };
}
