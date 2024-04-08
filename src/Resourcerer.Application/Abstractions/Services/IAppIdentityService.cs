using System.Security.Claims;

namespace Resourcerer.Application.Abstractions.Services;

public interface IAppIdentityService<T> where T : class, new()
{
    void Set(T identity);
    void Set(IEnumerable<Claim> claims);
    T Get();
}
