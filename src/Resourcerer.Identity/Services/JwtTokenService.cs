using Microsoft.IdentityModel.Tokens;
using Resourcerer.Identity.Constants;
using Resourcerer.Identity.Models;
using Resourcerer.Identity.Utils;
using System.Security.Claims;

namespace Resourcerer.Identity.Services;

public class JwtTokenService
{
    private readonly SymmetricSecurityKey _secretKey;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly byte _tokenExpirationMinutes = 60;

    public JwtTokenService(SymmetricSecurityKey secretKey, string issuer, string audience)
    {
        _secretKey = secretKey;
        _issuer = issuer;
        _audience = audience;
    }

    public string GenerateToken(
        AppIdentity identity,
        Dictionary<string, string[]> permissionMap,
        string displayName,
        string companyName)
    {
        var claims = new Dictionary<string, object>
        {
            [Claims.ClaimId] = identity.Id.ToString(),
            [Claims.ClaimUsername] = identity.Name!,
            [Claims.ClaimEmail] = identity.Email!.ToString(),
            [Claims.ClaimIsAdmin] = identity.Admin.ToString(),
            [Claims.ClaimCompanyId] = identity.CompanyId.ToString(),

            [Claims.ClaimDisplayName] = displayName,
            [Claims.ClaimCompanyName] = companyName
        };

        Permissions.AddClaimsFromPermissionsMap(claims, permissionMap);

        return WriteToken(claims);
    }

    public string RefreshToken(IEnumerable<Claim> claims) => WriteToken(claims);

    private string WriteToken(Dictionary<string, object> claims)
    {
        var creadentials = new SigningCredentials(_secretKey, SecurityAlgorithms.HmacSha256);
        var now = DateTime.UtcNow;

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = _issuer,
            Audience = _audience,
            Claims = claims,
            IssuedAt = now,
            NotBefore = now,
            Expires = now.AddMinutes(120),
            SigningCredentials = creadentials
        };

        var handler = new Microsoft.IdentityModel.JsonWebTokens.JwtSecurityTokenHandler();

        var tokenString = handler.CreateToken(descriptor);

        var token = new JwtSecurityToken(
            _issuer,
            _audience,
            claims,
            now,
            now.AddMinutes(_tokenExpirationMinutes),
            creadentials);

        var handler = new JwtSecurityTokenHandler();

        return handler.WriteToken(token);
        return "";
    }
}
