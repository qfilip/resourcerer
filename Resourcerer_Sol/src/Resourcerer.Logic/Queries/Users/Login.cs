using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos;
using Resourcerer.Utilities.Cryptography;
using System.Text.Json;

namespace Resourcerer.Logic.Queries.Users;

public static class Login
{
    public static AppUser FakeAdmin = new AppUser
    {
        Id = Guid.NewGuid(),
        Name = "string",
        PasswordHash = Hasher.GetSha256Hash("string"),
        Permissions = JsonSerializer.Serialize(Permission.GetAllPermissionsDictionary())
    };

    public class Handler : IAppHandler<AppUserDto, AppUserDto>
    {
        public Task<HandlerResult<AppUserDto>> Handle(AppUserDto request)
        {
            if(request.Name == FakeAdmin.Name && Hasher.GetSha256Hash(request.Password!) == FakeAdmin.PasswordHash)
            {
                var claimsDict = JsonSerializer.Deserialize<Dictionary<string, string>>(FakeAdmin.Permissions!);
                claimsDict!.Add("sub", FakeAdmin.Id.ToString());
                claimsDict!.Add("name", FakeAdmin.Name!);
                claimsDict!.Add("pwdhash", FakeAdmin.PasswordHash!);

                var dto = new AppUserDto
                {
                    Name = FakeAdmin.Name,
                    Claims = Permission.GetClaimsFromDictionary(claimsDict!)
                };

                return Task.FromResult(HandlerResult<AppUserDto>.Ok(dto));
            }
            else
            {
                return Task.FromResult(HandlerResult<AppUserDto>.NotFound());
            }
        }
    }
}

