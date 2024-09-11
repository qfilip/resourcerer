using Resourcerer.DataAccess.Entities.JsonEntities;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Utilities;

namespace Resourcerer.DataAccess.Utilities.Faking;

public static partial class DF
{
    public static DateTime Now = new DateTime(2000, 1, 1);
    public static string MakeName() => $"test-{Guid.NewGuid()}";
    public static string MakeEmail() => $"{MiniId.Generate(5)}@notmail.com";
    public static T MakeEntity<T>(Func<T> retn) where T : AppDbEntity
    {
        var e = retn();
        if (e is AppDbJsonField jeb)
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
}
