using Resourcerer.DataAccess.Entities;

namespace Resourcerer.UnitTests.Utilities.Faker;

internal static partial class DF
{
    internal static IDictionary<Type, (Delegate, Type[])> FakingFunctions = LoadFakingFunctions();

    internal static IDictionary<Type, (Delegate, Type[])> LoadFakingFunctions()
    {
        return new Dictionary<Type, (Delegate, Type[])>()
        {
            {
                typeof(Company),
                (() => MakeEntity(() => new Company { Name = MakeName() }),
                Array.Empty<Type>())
            },
            {
                typeof(AppUser),
                (() => MakeEntity(() => new AppUser { Name = MakeName() }),
                [typeof(Company)])
            },
            {
                typeof(Category),
                (() => MakeEntity(() => new Category { Name = MakeName() }),
                [typeof(Company)])
            },
            {
                typeof(UnitOfMeasure),
                (() => MakeEntity(() => new UnitOfMeasure
                {
                    Name = MakeName(),
                    Symbol = "uom"
                }),
                [typeof(Company)])
            },
            {
                typeof(Price),
                (() => MakeEntity(() => new Price { UnitValue = 1 }),
                [typeof(Item)])
            },
            {
                typeof(Excerpt),
                (() => MakeEntity(() => new Excerpt { Quantity = 1 }),
                [typeof(Item)])
            },
            {
                typeof(Item),
                (() => MakeEntity(() => new Item
                {
                    Name = MakeName(),
                    ExpirationTimeSeconds = 1200,
                    ProductionTimeSeconds = 10,
                }),
                [typeof(Category), typeof(UnitOfMeasure)])
            },
            {
                typeof(ItemProductionOrder),
                (() => MakeEntity(() => new ItemProductionOrder { Quantity = 1 }),
                [typeof(Item)])
            },
            {
                typeof(Instance),
                (() => MakeEntity(() => new Instance { Quantity = 1 }),
                [typeof(Company), typeof(Item)])
            },
            {
                typeof(InstanceOrderedEvent),
                (() => MakeEntity(()=> new InstanceOrderedEvent { Quantity = 1 }),
                [typeof(Instance)])
            },
            {
                typeof(InstanceReservedEvent),
                (() => MakeEntity(()=> new InstanceReservedEvent { Quantity = 1 }),
                [typeof(Instance)])
            },
            {
                typeof(InstanceDiscardedEvent),
                (() => MakeEntity(()=> new InstanceDiscardedEvent { Quantity = 1 }),
                [typeof(Instance)])
            }
        };
    }

    internal static T Fake<T>(TestDbContext ctx, Action<T>? modifier = null) where T : AppDbEntity
    {
        var entity = Fake(typeof(T)) as T;
        modifier?.Invoke(entity!);

        var dbSet = ctx.Set<T>();
        dbSet.Add(entity!);

        return entity!;
    }

    internal static object Fake(Type t)
    {
        var hasKey = FakingFunctions.TryGetValue(t, out var tuple);
        if (!hasKey)
        {
            throw new Exception($"Faking function for type {t}, not found");
        }

        var fakingFunction = tuple.Item1;
        var relationalTypes = tuple.Item2;

        var entity = fakingFunction.DynamicInvoke();
        
        if(entity == null)
        {
            throw new Exception($"Faking function for type {t} returns null");
        }

        var entityType = entity.GetType();
        if(entityType != t)
        {
            throw new Exception($"Faking function for type {t} returns {entityType}");
        }

        foreach (var relationalType in relationalTypes)
        {
            var relationalProperties = t.GetProperties()
                .Where(x => x.PropertyType == relationalType)
                .ToArray();

            if(relationalProperties.Length == 0)
            {
                throw new Exception($"Invalid mapping of relational property array. Relational type {relationalType} doesn't exist on entity type {t}");
            }

            foreach(var relationalProperty in relationalProperties)
            {
                var relationPropertyValue = relationalProperty.GetValue(entity);
                if(relationPropertyValue == null)
                {
                    relationalProperty.SetValue(entity, Fake(relationalProperty.PropertyType));
                }
            }
        }

        return entity;
    }
}
