using System.Security.Claims;

namespace Resourcerer.Identity.Abstractions;

public interface IAppIdentityService<T>
{
    void Set(T identity);
    void Set(IEnumerable<Claim> claims);
    T Get();
}
