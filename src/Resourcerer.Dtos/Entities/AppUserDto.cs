using Resourcerer.DataAccess.Entities;
using Resourcerer.Identity.Utils;

namespace Resourcerer.Dtos.Entities;
public class AppUserDto : EntityDto
{
    public string? Name { get; set; }
    public string? DisplayName { get; set; }
    public string? Email { get; set; }
    public bool IsAdmin { get; set; }
    public string? Password { get; set; }
    public Dictionary<string, string[]>? PermissionsMap { get; set; }

    // relational
    public Guid CompanyId { get; set; }
    public CompanyDto? Company { get; set; }

    public static AppUserDto MapForJwt(AppUser entity)
    {
        return new AppUserDto
        {
            Id = entity.Id,
            Name = entity.Name,
            DisplayName = entity.DisplayName,
            Email = entity.Email,
            IsAdmin = entity.IsAdmin,
            CompanyId = entity.CompanyId,
            Company = new CompanyDto
            {
                Id = entity.Company!.Id,
                Name = entity.Company!.Name,
            },
            PermissionsMap = Permissions.GetPermissionsMap(entity.Permissions!)
        };
    }
}



