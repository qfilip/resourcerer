using System.Security.Claims;
using System.Text.Json;

namespace Resourcerer.Dtos;

public class Permissions
{
    private static List<ePermission> AllPermissions = Enum.GetValues<ePermission>().ToList();
    private static List<string> AllSections = Enum.GetValues<ePermissionSection>()
        .Select(x => x.ToString())
        .ToList();

    public static List<string> Validate(Dictionary<string, int> permissions)
    {
        var errors = new List<string>();
        var lookup = permissions.ToLookup(x => x.Key);

        foreach(var l in lookup)
        {
            if(!AllSections.Contains(l.Key))
            {
                errors.Add($"Permission of type {l.Key} doesn't exist");
            }
        }

        return errors;
    }

    public static Dictionary<string, int> GetAllPermissionsDictionary()
    {
        var dict = new Dictionary<string, int>();
        AllSections.ForEach(s =>
        {
            var level = 0;
            AllPermissions.ForEach(p =>
            {
                level = level | (int)p;
            });
            dict.Add(s, level);
        });

        return dict;
    }

    public static Dictionary<string, int> GetPermissionDictFromString(string permissionsJson)
    {
        return JsonSerializer.Deserialize<Dictionary<string, int>>(permissionsJson)!;
    }

    public static List<Claim> GetClaimsFromDictionary(Dictionary<string, int> permissionsDict)
    {
        var setPermissions = permissionsDict
            .Where(x => AllSections.Contains(x.Key))
            .ToList();

        var claims = new List<Claim>();
        setPermissions.ForEach(x =>
        {
            AllPermissions.ForEach(p =>
            {
                if (((int)p & x.Value) > 0)
                    claims.Add(new Claim(x.Key.ToString(), p.ToString()));
            });
        });
        
        return claims;
    }

    public static Dictionary<string, int> GetPermissionDictionaryFromClaims(List<Claim> claims)
    {
        var lookup = claims
            .Where(x => AllSections.Contains(x.Type))
            .ToLookup(x => x.Type);

        var permissionsDict = new Dictionary<string, int>();

        foreach (var l in lookup)
        {
            var setClaims = lookup[l.Key];
            var level = 0;
            
            foreach (var sc in setClaims)
            {
                var e = Enum.Parse<ePermission>(sc.Value);
                level = level | (int)e;
            }

            permissionsDict.Add(l.Key, level);
        }

        return permissionsDict;
    }
}

[Flags]
public enum ePermission
{
    Read = 1,
    Write = 2,
    Modify = 4,
    Remove = 8
}

public enum ePermissionSection
{
    User,
    Category,
    Element,
    Event
}
