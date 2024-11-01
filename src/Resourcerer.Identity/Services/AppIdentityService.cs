using Resourcerer.Identity.Abstractions;
using Resourcerer.Identity.Constants;
using Resourcerer.Identity.Models;
using System.Security.Claims;

namespace Resourcerer.Identity.Services;

public sealed class AppIdentityService : IAppIdentityService<AppIdentity>
{
    private delegate bool TryParse<TData>(string str, out TData value);

    private AppIdentity? _identity;
    private readonly bool _authEnabled;
    private readonly AppIdentity _systemIdentity;

    public AppIdentityService(bool authEnabled, AppIdentity systemIdentity)
    {
        _authEnabled = authEnabled;
        _systemIdentity = systemIdentity;
    }

    public AppIdentity Get() => _identity ?? _systemIdentity;
    public void Set(AppIdentity identity) => _identity = identity;
    public void Set(IEnumerable<Claim> claims)
    {
        if (!_authEnabled) return;

        var id = GetClaim<Guid>(claims, Claims.ClaimId, Guid.TryParse);
        var name = GetClaim<string>(claims, Claims.ClaimUsername, Return);
        var email = GetClaim<string>(claims, Claims.ClaimEmail, Return);
        var isAdmin = GetClaim<bool>(claims, Claims.ClaimIsAdmin, bool.TryParse);
        var companyId = GetClaim<Guid>(claims, Claims.ClaimCompanyId, Guid.TryParse);

        _identity = new(id, name!, email!, isAdmin, companyId);
    }

    private static T? GetClaim<T>(
        IEnumerable<Claim> claims,
        string type,
        TryParse<T> parser,
        bool optional = false
    )
    {
        var claim = claims.FirstOrDefault(x => x.Type == type);

        if (claim == null)
        {
            if (!optional)
            {
                throw new Exception($"Couldn't find claim of type {type}");
            }

            return default;
        }

        var parsed = parser(claim.Value, out var value);
        if (!parsed)
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
