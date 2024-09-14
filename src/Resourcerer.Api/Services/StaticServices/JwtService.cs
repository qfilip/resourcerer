using Microsoft.IdentityModel.Tokens;
using Resourcerer.Application.Auth.Abstractions;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos;
using Resourcerer.Dtos.Entity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Resourcerer.Api.Services.StaticServices;

public class JwtService
{
    public static string GenerateToken(AppUserDto dto)
    {
        var claims = new List<Claim>
        {
            new Claim(IAppIdentityService<AppUser>.ClaimId, dto.Id.ToString()),
            new Claim(IAppIdentityService<AppUser>.ClaimUsername, dto.Name!),
            new Claim(IAppIdentityService<AppUser>.ClaimEmail, dto.Email!.ToString()),
            new Claim(IAppIdentityService<AppUser>.ClaimDisplayName, dto.DisplayName!.ToString()),
            new Claim(IAppIdentityService<AppUser>.ClaimIsAdmin, dto.IsAdmin.ToString()),
            new Claim(IAppIdentityService<AppUser>.ClaimCompanyId, dto.Company!.Id.ToString()),
            new Claim(IAppIdentityService<AppUser>.ClaimCompanyName, dto.Company!.Name!.ToString())
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
