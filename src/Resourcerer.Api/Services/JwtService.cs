using Microsoft.IdentityModel.Tokens;
using Resourcerer.Dtos;
using Resourcerer.Dtos.Entity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Resourcerer.Api.Services;

public class JwtService
{
    public static string GenerateToken(AppUserDto dto)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Email, dto.Email!.ToString()),
            new Claim(AppStaticData.Auth.Jwt.UserName, dto.Name!),
            new Claim(AppStaticData.Auth.Jwt.UserId, dto.Id.ToString()),
            new Claim(AppStaticData.Auth.Jwt.DisplayName, dto.DisplayName!.ToString()),
            new Claim(AppStaticData.Auth.Jwt.IsAdmin, dto.IsAdmin.ToString()),
            new Claim(AppStaticData.Auth.Jwt.CompanyId, dto.Company!.Id.ToString()),
            new Claim(AppStaticData.Auth.Jwt.CompanyName, dto.Company!.Name!.ToString())
        };

        claims.AddRange(Permissions.GetClaimsFromPermissionsMap(dto.PermissionsMap!));

        return WriteToken(claims);
    }

    public static string RefreshToken(IEnumerable<Claim> claims) => WriteToken(claims);

    private static string WriteToken(IEnumerable<Claim> claims)
    {
        var creadentials = new SigningCredentials(AppStaticData.Auth.Jwt.Key, SecurityAlgorithms.HmacSha256);
        var now = DateTime.UtcNow;

        var token = new JwtSecurityToken(
            AppStaticData.Auth.Jwt.Issuer,
            AppStaticData.Auth.Jwt.Audience,
            claims,
            now,
            now.AddMinutes(30),
            creadentials);

        var handler = new JwtSecurityTokenHandler();

        return handler.WriteToken(token);
    }
}
