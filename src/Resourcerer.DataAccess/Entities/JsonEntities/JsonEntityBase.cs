using Resourcerer.Utilities;

namespace Resourcerer.DataAccess.Entities.JsonEntities;

public class JsonEntityBase : EntityBase
{
    public new string? Id { get; set; }
    public static T CreateEntity<T>(Func<T> generator) where T : JsonEntityBase
    {
        var t = generator();
        var now = DateTime.UtcNow;

        t.Id = MiniId.Generate();
        t.CreatedAt = now;
        t.ModifiedAt = now;
        t.EntityStatus = Enums.eEntityStatus.Active;

        return t;
    }
}
