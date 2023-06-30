using Microsoft.IdentityModel.Tokens;
using Resourcerer.Dtos.Users;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Resourcerer.Api.Services;

public class JwtService
{
    public string GenerateToken(UserDto dto)
    {
        var claims = new[]
        {
                new Claim(JwtRegisteredClaimNames.Sub, dto.Name!),
                //new Claim(ClaimTypes.Role, user.Role)
        };

        var creadentials = new SigningCredentials(AppStaticData.Jwt.Key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            AppStaticData.Jwt.Issuer,
            AppStaticData.Jwt.Audience,
            claims,
            DateTime.UtcNow,
            DateTime.UtcNow.AddMinutes(30),
            creadentials);

        var handler = new JwtSecurityTokenHandler();

        return handler.WriteToken(token);
    }
}
