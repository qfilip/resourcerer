using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos;
using Resourcerer.Dtos.Entity;
using System.Text.Json;

namespace Resourcerer.Logic;

public static partial class Mapper
{
    public static AppUser Map(AppUserDto src) =>
        Map(() =>
            new AppUser
            {
                Name = src.Name,
                DisplayName = src.DisplayName,
                Email = src.Email,
                IsAdmin = src.IsAdmin,
                PasswordHash = src.Password,
                Permissions = JsonSerializer.Serialize(Permissions.GetCompressedFrom(src.PermissionsMap!)),
                
                CompanyId = src.CompanyId,
                Company = Map(src.Company, Map)
            }, src);

    public static AppUserDto Map(AppUser src) =>
        Map(() =>
            new AppUserDto
            {
                Name = src.Name,
                DisplayName = src.DisplayName,
                Email = src.Email,
                IsAdmin = src.IsAdmin,
                Password = src.PasswordHash,
                PermissionsMap = Permissions.GetPermissionsMap(src.Permissions!),

                CompanyId = src.CompanyId,
                Company = Map(src.Company, Map)
            }, src);
}
