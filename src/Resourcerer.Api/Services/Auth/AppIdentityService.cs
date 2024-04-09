﻿using Resourcerer.Application.Abstractions.Services;
using Resourcerer.DataAccess.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Resourcerer.Api.Services.Auth;

public sealed class AppIdentityService : IAppIdentityService<AppUser>
{
    private delegate bool TryParse<T>(string str, out T value);
    
    private AppUser? _user;
    
    public AppUser Get() => _user is null ? new AppUser() : _user;
    public void Set(AppUser identity) => _user = identity;
    public void Set(IEnumerable<Claim> claims)
    {
        var id = GetClaim<Guid>(claims, AppStaticData.Auth.Jwt.UserId, Guid.TryParse);
        var name = GetClaim<string>(claims, AppStaticData.Auth.Jwt.UserName, Return);
        var email = GetClaim<string>(claims, JwtRegisteredClaimNames.Email, Return);
        var isAdmin = GetClaim<bool>(claims, AppStaticData.Auth.Jwt.IsAdmin, bool.TryParse);
        var companyId = GetClaim<Guid>(claims, AppStaticData.Auth.Jwt.CompanyId, Guid.TryParse);

        _user = new AppUser
        {
            Id = id,
            Name = name,
            Email = email,
            IsAdmin = isAdmin,
            CompanyId = companyId,
        };
    }

    private static T? GetClaim<T>(
        IEnumerable<Claim> claims,
        string type,
        TryParse<T> parser,
        bool optional = false
    )
    {
        var claim = claims.FirstOrDefault(x => x.Type == type);

        if( claim == null )
        {
            if(!optional)
            {
                throw new Exception($"Couldn't find claim of type {type}");
            }

            return default;
        }
        
        var parsed = parser(claim.Value, out var value);
        if(!parsed)
        {
            throw new Exception($"Couldn't parse claim of type {type}");
        }

        return value;
    }

    private static bool Return(string src, out string dest)
    {
        dest = src;
        return true;
    }
}
