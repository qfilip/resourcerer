using Resourcerer.DataAccess.Entities;
using System.Security.Claims;
using System.Text.Json;

namespace Resourcerer.Dtos;

public class Permission
{
    private static List<ePermission> AllPermissions = Enum.GetValues<ePermission>().ToList();
    private static List<string> AllSections = Enum.GetValues<eSection>()
        .Select(x => x.ToString())
        .ToList();

    public static List<string> Validate(Dictionary<string, string> permissions)
    {
        var errors = new List<string>();
        var lookup = permissions.ToLookup(x => x.Key);

        foreach(var l in lookup)
        {
            if(!AllSections.Contains(l.Key))
            {
                errors.Add($"Permission of type {l.Key} doesn't exist");
            }
            else
            {
                foreach (var kv in lookup[l.Key])
                {
                    if(!Enum.TryParse(kv.Value, out ePermission p))
                    {
                        errors.Add($"Permission of value {p} doesn't exist");
                    }
                }
            }
        }

        return errors;
    }

    public static Dictionary<string, string> GetAllPermissionsDictionary()
    {
        var dict = new Dictionary<string, string>();
        AllSections.ForEach(s =>
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

    public static List<Claim> GetClaimsFromString(string permissionsJson)
    {
        var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(permissionsJson)!;
        return GetClaimsFromDictionary(dict);
    }

    public static List<Claim> GetClaimsFromDictionary(Dictionary<string, string> permissionsDict)
    {
        var setPermissions = permissionsDict
            .Where(x => AllSections.Contains(x.Key))
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
            .Where(x => AllSections.Contains(x.Type))
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

public enum eSection
{
    User,
    Category,
    Item
}
