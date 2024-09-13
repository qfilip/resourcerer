using System.Security.Claims;

namespace Resourcerer.Application.Auth.Abstractions;

public interface IAppIdentityService<T> where T : class, new()
{
    const string ClaimId = "user_id";
    const string ClaimUsername = "user_name";
    const string ClaimEmail = "user_email";
    const string ClaimDisplayName = "user_display_name";
    const string ClaimIsAdmin = "user_is_admin";
    const string ClaimCompanyId = "user_company_id";
    const string ClaimCompanyName = "user_company_name";

    void Set(T identity);
    void Set(IEnumerable<Claim> claims);
    T Get();
}
