using Resourcerer.DataAccess.Entities;

namespace Resourcerer.Dtos.Entity;
public class AppUserDto : EntityDto<AppUserDto>
{
    public string? Name { get; set; }
    public string? DisplayName { get; set; }
    public string? Email { get; set; }
    public bool IsAdmin { get; set; }
    public string? Password { get; set; }
    public Dictionary<string, string[]>? PermissionsMap { get; set; }

    // relational
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
            Company = new CompanyDto
            {
                Id = entity.Company!.Id,
                Name = entity.Company!.Name,
            },
            PermissionsMap = Permissions.GetPermissionsMap(entity.Permissions!)
        };
    }
}



