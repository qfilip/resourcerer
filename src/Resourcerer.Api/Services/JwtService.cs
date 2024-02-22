using Microsoft.IdentityModel.Tokens;
using Resourcerer.Dtos;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;

namespace Resourcerer.Api.Services;

public class JwtService
{
    public static string GenerateToken(AppUserDto dto)
    {
        var claims = Permissions.GetClaimsFromDictionary(dto.Permissions!);
        claims.Add(new Claim(JwtRegisteredClaimNames.Sub, dto.Name!));

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
