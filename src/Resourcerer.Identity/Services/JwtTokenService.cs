using Microsoft.IdentityModel.Tokens;
using Resourcerer.Identity.Constants;
using Resourcerer.Identity.Models;
using Resourcerer.Identity.Utils;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Resourcerer.Identity.Services;

public class JwtTokenService
{
    private readonly SymmetricSecurityKey _secretKey;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly byte _tokenExpirationTimeSeconds;

    public JwtTokenService(SymmetricSecurityKey secretKey, string issuer, string audience, byte tokenExpirationTimeSeconds)
    {
        _secretKey = secretKey;
        _issuer = issuer;
        _audience = audience;
        _tokenExpirationTimeSeconds = tokenExpirationTimeSeconds;
    }

    public string GenerateToken(
        AppIdentity identity,
        Dictionary<string, string[]> permissionMap,
        string displayName,
        string companyName)
    {
        var claims = new List<Claim>
        {
            new Claim(Claims.ClaimId, identity.Id.ToString()),
            new Claim(Claims.ClaimUsername, identity.Name!),
            new Claim(Claims.ClaimEmail, identity.Email!.ToString()),
            new Claim(Claims.ClaimIsAdmin, identity.Admin.ToString()),
            new Claim(Claims.ClaimCompanyId, identity.CompanyId.ToString()),

            new Claim(Claims.ClaimDisplayName, displayName),
            new Claim(Claims.ClaimCompanyName, companyName)
        };

        claims.AddRange(Permissions.GetClaimsFromPermissionsMap(permissionMap));

        return WriteToken(claims);
    }

    public string RefreshToken(IEnumerable<Claim> claims) => WriteToken(claims);

    private string WriteToken(IEnumerable<Claim> claims)
    {
        var creadentials = new SigningCredentials(_secretKey, SecurityAlgorithms.HmacSha256);
        var now = DateTime.UtcNow;

        var token = new JwtSecurityToken(
            _issuer,
            _audience,
            claims,
            now,
            now.AddSeconds(_tokenExpirationTimeSeconds),
            creadentials);

        var handler = new JwtSecurityTokenHandler();

        return handler.WriteToken(token);
    }
}