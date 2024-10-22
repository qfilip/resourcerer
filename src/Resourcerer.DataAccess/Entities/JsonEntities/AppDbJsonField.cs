using Resourcerer.DataAccess.Abstractions;
using Resourcerer.DataAccess.Records;
using Resourcerer.Utilities;

namespace Resourcerer.DataAccess.Entities.JsonEntities;

public sealed class AppDbJsonField
{   
    public static T Create<T>(Func<T> generator)
        where T : IId<string>, IAuditedEntity<ReadOnlyAudit>
    {
        var t = generator();

        t.Id = MiniId.Generate();
        t.AuditRecord = new ReadOnlyAudit { CreatedAt = DateTime.UtcNow };

        return t;
    }

    public static T Create<T>(Action<T>? modifier = null)
        where T : IId<string>, IAuditedEntity<ReadOnlyAudit>, new()
    {
        var t = new T();

        t.Id = MiniId.Generate();
        t.AuditRecord = new ReadOnlyAudit { CreatedAt = DateTime.UtcNow };

        modifier?.Invoke(t);

        return t;
    }

    public static T CreateKeyless<T>(Func<T> generator)
        where T : IAuditedEntity<ReadOnlyAudit>
    {
        var t = generator();

        t.AuditRecord = new ReadOnlyAudit { CreatedAt = DateTime.UtcNow };

        return t;
    }

    public static T CreateKeyless<T>(Action<T>? modifier = null)
        where T : IAuditedEntity<ReadOnlyAudit>, new()
    {
        var t = new T();

        t.AuditRecord = new ReadOnlyAudit { CreatedAt = DateTime.UtcNow };

        modifier?.Invoke(t);

        return t;
    }
}
