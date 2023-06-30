﻿using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Resourcerer.Api.Services;

public class JwtService
{
    public static string GenerateToken(List<Claim> claims)
    {
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
