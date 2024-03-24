using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Entities.JsonEntities;
using Resourcerer.DataAccess.Enums;
using Resourcerer.Dtos;
using Resourcerer.Utilities;
using Resourcerer.Utilities.Cryptography;
using System.Text.Json;

namespace Resourcerer.UnitTests.Utilities.Faker;

internal static partial class DF
{
    public static DateTime Now = new DateTime(2000, 1, 1);
    public static string MakeName() => $"test-{Guid.NewGuid().ToString("n").Substring(0, 6)}";
    public static T MakeEntity<T>(Func<T> retn) where T : AppDbEntity
    {
        var e = retn();
        if(e is AppDbJsonField jeb)
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
