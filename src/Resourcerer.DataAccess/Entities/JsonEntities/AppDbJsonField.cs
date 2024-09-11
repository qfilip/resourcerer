using Resourcerer.Utilities;

namespace Resourcerer.DataAccess.Entities.JsonEntities;

public class AppDbJsonField
{
    public string? Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
    
    public static T Create<T>(Func<T> generator) where T : AppDbJsonField
    {
        var t = generator();
        var now = DateTime.UtcNow;

        t.Id = MiniId.Generate();
        t.CreatedAt = now;
        t.ModifiedAt = now;

        return t;
    }

    public static T Create<T>(Action<T>? modifier = null) where T : AppDbJsonField, new()
    {
        var t = new T();
        var now = DateTime.UtcNow;

        t.Id = MiniId.Generate();
        t.CreatedAt = now;
        t.ModifiedAt = now;
        
        modifier?.Invoke(t);

        return t;
    }
}
