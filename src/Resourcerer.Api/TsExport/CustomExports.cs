using Resourcerer.Identity.Constants;
using System.Text;

namespace Resourcerer.Api.TsExport;

public static class CustomExports
{
    public static void ExportJwtClaimKeys(StringBuilder sb)
    {
        sb.Append("export const jwtClaimKeys = {");
        sb.Append(Environment.NewLine);
        var xs = new List<(string Key, string Value)>()
        {
            ("id", Claims.ClaimId),
            ("name", Claims.ClaimUsername),
            ("displayName", Claims.ClaimUsername),
            ("email", Claims.ClaimEmail)
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
