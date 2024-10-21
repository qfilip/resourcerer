using Resourcerer.DataAccess.Abstractions;
using Resourcerer.DataAccess.Entities.JsonEntities;
using Resourcerer.Utilities;

namespace Resourcerer.DataAccess.Utilities.Faking;

public static partial class DF
{
    public static DateTime Now = new DateTime(2000, 1, 1);
    public static string MakeName() => $"test-{Guid.NewGuid()}";
    public static string MakeEmail() => $"{MiniId.Generate(5)}@notmail.com";
    public static T MakeEntity<T>(Func<T> retn) where T : class, IAuditedEntity, ISoftDeletable
    {
        var e = retn();
        if (e is AppDbJsonField jeb)
        {
            jeb.Id = jeb.Id ?? MiniId.Generate();
        }
        else if(e is IId<Guid> epk)
        {
            epk.Id = epk.Id != Guid.Empty ? epk.Id : Guid.NewGuid();
        }

        e.AuditRecord = new()
        {
            CreatedAt = Now,
            ModifiedAt = Now
        };

        return e;
    }
}
