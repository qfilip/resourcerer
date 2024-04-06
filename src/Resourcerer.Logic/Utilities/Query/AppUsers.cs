using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos;
using Resourcerer.Dtos.Entity;
using System.Linq.Expressions;

namespace Resourcerer.Logic.Utilities.Query;

public class AppUsers
{
    public static Expression<Func<AppUser, AppUserDto>> DefaultDtoProjection =
        EntityBases.Expand<AppUser, AppUserDto>(x => new AppUserDto
        {
            Id = x.Id,
            Name = x.Name,
            Company = new()
            {
                Id = x.Company!.Id,
                Name = x.Company.Name
            },
            PermissionsMap = Permissions.GetPermissionsMap(x.Permissions!)
        });
}
