﻿using Resourcerer.Dtos;
using System.IdentityModel.Tokens.Jwt;
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
            ("name", JwtRegisteredClaimNames.Sub),
            ("issuedAt", JwtRegisteredClaimNames.Iat),
            ("userId", AppStaticData.Auth.Jwt.UserId),
            ("isAdmin", AppStaticData.Auth.Jwt.Admin),
            ("companyId", AppStaticData.Auth.Jwt.CompanyId),
            ("companyName", AppStaticData.Auth.Jwt.CompanyName)
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