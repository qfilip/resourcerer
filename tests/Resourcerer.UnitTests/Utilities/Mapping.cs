using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.Entity;
using Resourcerer.Identity.Models;

namespace Resourcerer.UnitTests.Utilities;

public static class Mapping
{
    internal static AppIdentity Of(AppUser user) =>
        new(user.Id, user.Name!, user.Email!, user.IsAdmin, user.CompanyId);

    internal static AppIdentity Of(AppUserDto user) =>
        new(user.Id, user.Name!, user.Email!, user.IsAdmin, user.CompanyId);
}
