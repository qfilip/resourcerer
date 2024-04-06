using System.Reflection.Emit;
using System.Security.Claims;
using System.Text.Json;

namespace Resourcerer.Dtos;

public class Permissions
{
    public readonly static List<ePermission> AllPermissions = Enum.GetValues<ePermission>()
        .ToList();
    
    public readonly static List<string> AllSections = Enum.GetValues<ePermissionSection>()
        .Select(x => x.ToString())
        .ToList();

    public static List<string> Validate(Dictionary<string, string[]> permissionsMap)
    {
        var errors = new List<string>();

        foreach(var kv in permissionsMap)
        {
            if(!Enum.TryParse<ePermissionSection>(kv.Key, out var _))
            {
                errors.Add($"Permission section {kv.Key} doesn't exist");
            }

            foreach(var permission in  kv.Value)
            {
                if(!Enum.TryParse<ePermission>(permission, out var _))
                    errors.Add($"Permission {permission} doesn't exist");
            }
        }

        return errors;
    }

    public static Dictionary<string, int> GetCompressed()
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

    public static Dictionary<string, int> GetCompressedFrom(string permissionsJson)
    {
        return JsonSerializer.Deserialize<Dictionary<string, int>>(permissionsJson)!;
    }

    public static Dictionary<string, int> GetCompressedFrom(Dictionary<string, string[]> permissionsMap)
    {
        var compressed = new Dictionary<string, int>();
        foreach(var kv in permissionsMap)
        {
            var level = 0;
            foreach (var permission in kv.Value)
            {
                var e = Enum.Parse<ePermission>(permission);
                level = level | (int)e;
            }

            compressed.Add(kv.Key, level);
        }

        return compressed;
    }

    public static Dictionary<string, string[]> GetPermissionsMap(string permissionsJson)
    {
        var compressed = GetCompressedFrom(permissionsJson);
        return GetPermissionsMap(compressed);
    }

    public static Dictionary<string, string[]> GetPermissionsMap(Dictionary<string, int> compressed)
    {
        var dict = new Dictionary<string, string[]>();

        var setPermissions = compressed
            .Where(x => AllSections.Contains(x.Key))
            .ToList();

        var claims = new List<Claim>();
        setPermissions.ForEach(x =>
        {
            var permissions = AllPermissions
                .Select(p =>
                {
                    if (((int)p & x.Value) > 0)
                    {
                        return p.ToString();
                    }
                    else
                    {
                        return null;
                    }
                })
                .OfType<string>()
                .ToArray();

            dict.Add(x.Key.ToString(), permissions);
        });

        return dict;
    }

    public static List<Claim> GetClaimsFromPermissionsMap(Dictionary<string, string[]> permissionsMap)
    {
        var claims = new List<Claim>();

        foreach (var kv in permissionsMap)
            foreach (var v in kv.Value)
                claims.Add(new Claim(kv.Key, v));

        return claims;
    }

    public static List<Claim> GetClaimsFromCompressed(Dictionary<string, int> compressed)
    {
        var setPermissions = compressed
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

    public static Dictionary<string, int> GetCompressedFromClaims(List<Claim> claims)
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
