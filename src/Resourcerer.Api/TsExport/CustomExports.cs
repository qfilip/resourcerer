using Resourcerer.Dtos;
using Resourcerer.Identity.Constants;
using Resourcerer.Identity.Utils;
using System.Text;

namespace Resourcerer.Api.TsExport;

public static class CustomExports
{
    public static void ExportPermissionsMapConst(StringBuilder sb)
    {
        var permissions = Permissions.AllPermissions.Select(x => $"'{x}'").ToArray();
        var permissionsArrayString = string.Join(',', permissions);

        sb.Append("export const permissionsMap: { [key:string]: string[] } = {");
        sb.Append(Environment.NewLine);
        Permissions.AllSections.ForEach(s =>
        {
            var kv = $"\t'{s}': [{permissionsArrayString}],";
            sb.Append(kv);
            sb.Append(Environment.NewLine);
        });
        sb.Append("};");
    }

    public static void ExportJwtClaimKeys(StringBuilder sb)
    {
        sb.Append("export const jwtClaimKeys = {");
        sb.Append(Environment.NewLine);
        var xs = new List<(string Key, string Value)>()
        {
            ("id", Claims.ClaimId),
            ("name", Claims.ClaimUsername),
            ("displayName", Claims.ClaimUsername),
            ("email", Claims.ClaimEmail),
            ("isAdmin", Claims.ClaimIsAdmin),
            ("companyId", Claims.ClaimCompanyId),
            ("companyName", Claims.ClaimCompanyName)
        };

        xs.ForEach(x =>
        {
            var kv = $"\t{x.Key}: '{x.Value}',";
            sb.Append(kv);
            sb.Append(Environment.NewLine);
        });
        sb.Append("};");
    }
}
