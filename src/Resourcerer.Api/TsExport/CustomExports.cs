using Resourcerer.Dtos;
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
}
