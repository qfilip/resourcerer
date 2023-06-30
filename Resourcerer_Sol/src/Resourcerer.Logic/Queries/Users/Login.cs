using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos;
using Resourcerer.Dtos.Users;
using Resourcerer.Utilities.Cryptography;
using System.Text.Json;

namespace Resourcerer.Logic.Queries.Users;

public static class Login
{
    public static AppUser FakeAdmin = new AppUser
    {
        Id = Guid.NewGuid(),
        Name = "admin",
        PasswordHash = Hasher.GetSha256Hash("admin"),
        Claims = JsonSerializer.Serialize(Permission.GetAllPermissionsDictionary())
    };

    public class Handler : IRequestHandler<UserDto, UserDto>
    {
        public Task<HandlerResult<UserDto>> Handle(UserDto request)
        {
            if(request.Name == FakeAdmin.Name && Hasher.GetSha256Hash(request.Password!) == FakeAdmin.PasswordHash)
            {
                var claimsDict = JsonSerializer.Deserialize<Dictionary<string, string>>(FakeAdmin.Claims!);
                
                var dto = new UserDto
                {
                    Name = FakeAdmin.Name,
                    Claims = Permission.GetClaimsFromDictionary(claimsDict!)
                };

                return Task.FromResult(HandlerResult<UserDto>.Ok(dto));
            }
            else
            {
                return Task.FromResult(HandlerResult<UserDto>.NotFound());
            }
        }
    }
}

