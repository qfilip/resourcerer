using Microsoft.IdentityModel.Tokens;
using Resourcerer.Dtos;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Resourcerer.Api.Services;

public class JwtService
{
    public static string GenerateToken(AppUserDto dto)
    {
        var claims = Permissions.GetClaimsFromDictionary(dto.Permissions!);
        claims.Add(new Claim(JwtRegisteredClaimNames.Sub, dto.Name!));
        
        var creadentials = new SigningCredentials(AppStaticData.Jwt.Key, SecurityAlgorithms.HmacSha256);
        var now = DateTime.UtcNow;

        var token = new JwtSecurityToken(
            AppStaticData.Jwt.Issuer,
            AppStaticData.Jwt.Audience,
            claims,
            now,
            // DateTime.UtcNow.AddMinutes(30),
            now.AddSeconds(5),
            creadentials);

        var handler = new JwtSecurityTokenHandler();

        return handler.WriteToken(token);
    }
}
