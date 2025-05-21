using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.Entities;
using Resourcerer.Identity.Utils;
using System.Linq.Expressions;

namespace Resourcerer.Logic.Utilities.Query;

public class AppUsers
{
    public static Expression<Func<AppUser, AppUser>> Expand(Expression<Func<AppUser, AppUser>> projectionLayer)
    {
        return ExpressionUtils.Combine(DefaultProjection, projectionLayer);
    }

    public static Expression<Func<AppUser, AppUser>> DefaultProjection =
        EntityBases.Expand<AppUser>(x => new AppUser
        {
            Id = x.Id,
            Name = x.Name,
            CompanyId = x.CompanyId,
            DisplayName = x.DisplayName,
            Email = x.Email,
            IsAdmin = x.IsAdmin,
            Permissions = x.Permissions,
            Company = new()
            {
                Id = x.Company!.Id,
                Name = x.Company.Name
            }
        });

    public static Expression<Func<AppUser, AppUserDto>> DefaultDtoProjection =
        EntityBases.Expand<AppUser, AppUserDto>(x => new AppUserDto
        {
            Id = x.Id,
            Name = x.Name,
            DisplayName = x.DisplayName,
            CompanyId = x.CompanyId,
            Email = x.Email,
            IsAdmin = x.IsAdmin,
            PermissionsMap = Permissions.GetPermissionsMap(x.Permissions!),
            Company = new()
            {
                Id = x.Company!.Id,
                Name = x.Company.Name
            }
        });
}
