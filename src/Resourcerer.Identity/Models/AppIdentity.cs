namespace Resourcerer.Identity.Models;
public sealed record AppIdentity(
    Guid Id,
    string Name,
    string Email,
    bool Admin,
    Guid CompanyId
);
