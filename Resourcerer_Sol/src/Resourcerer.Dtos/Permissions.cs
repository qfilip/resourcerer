using Resourcerer.DataAccess.Entities;
using System.Security.Claims;

namespace Resourcerer.Dtos;

public class Permission
{
    private static List<ePermission> AllPermissions = Enum.GetValues<ePermission>().ToList();
    private static List<string> AllSets = new List<string>
    {
        $"{nameof(AppUser)}s",
        $"{nameof(Category)}s",
        $"{nameof(Composite)}s",
        $"{nameof(CompositeSoldEvent)}s",
        $"{nameof(Element)}s",
        $"{nameof(ElementSoldEvent)}s",
        $"{nameof(Excerpt)}s",
        $"{nameof(Price)}s",
        $"{nameof(UnitOfMeasure)}s"
    };

    public static Dictionary<string, string> GetAllPermissionsDictionary()
    {
        var dict = new Dictionary<string, string>();
        AllSets.ForEach(s =>
        {
            var level = 0;
            AllPermissions.ForEach(p =>
            {
                level = level | (int)p;
            });
            dict.Add(s, level.ToString());
        });

        return dict;
    }

    public static List<Claim> GetClaimsFromDictionary(Dictionary<string, string> permissionsDict)
    {
        var setPermissions = permissionsDict
            .Where(x => AllSets.Contains(x.Key))
            .ToList();

        var claims = new List<Claim>();

        setPermissions.ForEach(x =>
        {
            var permissionInt = int.Parse(x.Value);
            AllPermissions.ForEach(p =>
            {
                var hasPermission = (permissionInt & (int)p) > 0;
                if(hasPermission)
                {
                    claims.Add(new Claim(x.Key, p.ToString()));
                }
            });
        });

        return claims;
    }

    public static Dictionary<string, string> GetPermissionDictionaryFromClaims(List<Claim> claims)
    {
        var lookup = claims
            .Where(x => AllSets.Contains(x.Type))
            .ToLookup(x => x.Type);

        var permissionsDict = new Dictionary<string, string>();

        foreach (var l in lookup)
        {
            var setClaims = lookup[l.Key];
            var level = 0;
            
            foreach (var sc in setClaims)
            {
                var e = Enum.Parse<ePermission>(sc.Value);
                level = level | (int)e;
            }

            permissionsDict.Add(l.Key, level.ToString());
        }

        return permissionsDict;
    }
}

[Flags]
public enum ePermission
{
    Read = 1,
    Write = 2,
    Remove = 4
}
