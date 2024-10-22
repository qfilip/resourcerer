using Resourcerer.DataAccess.Entities;
using Resourcerer.Utilities.Cryptography;

namespace Resourcerer.DataAccess.Utilities.Faking;

public static partial class DF
{
    public static Dictionary<Type, Delegate> FakingFunctions = new()
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
