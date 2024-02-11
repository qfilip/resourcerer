namespace Resourcerer.DataAccess.Entities.JsonEntities;

public class JsonEntityBase : EntityBase
{
    public static T CreateEntity<T>(Func<T> generator) where T : JsonEntityBase
    {
        var t = generator();
        var now = DateTime.UtcNow;
        
        t.Id = Guid.NewGuid();
        t.CreatedAt = now;
        t.ModifiedAt = now;
        t.EntityStatus = Enums.eEntityStatus.Active;

        return t;
    }
}
