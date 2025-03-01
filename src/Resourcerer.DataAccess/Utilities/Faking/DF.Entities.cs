using Resourcerer.DataAccess.Abstractions;
using Resourcerer.DataAccess.Entities.JsonEntities;
using Resourcerer.DataAccess.Records;
using Resourcerer.Utilities;

namespace Resourcerer.DataAccess.Utilities.Faking;

public static partial class DF
{
    public static DateTime Now = new DateTime(2000, 1, 1);
    public static string MakeName() => $"test-{Guid.NewGuid()}";
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

    public static T MakeEventEntity<T>(Func<T> retn)
        where T : IId<Guid>, IAuditedEntity<Audit>
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
}
