using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.Users;
using Resourcerer.Utilities.Cryptography;
using System.Text.Json;

namespace Resourcerer.Logic.Commands.Users;
public static class Register
{
    public class Handler : IRequestHandler<UserDto, UserDto>
    {
        private readonly AppDbContext _dbContext;

        public Handler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<HandlerResult<UserDto>> Handle(UserDto request)
        {
            if (string.IsNullOrEmpty(request.Name) || string.IsNullOrEmpty(request.Password))
            {
                return HandlerResult<UserDto>.ValidationError(new string[] { "Name or password cannot be empty" });
            }

            var claims = new Dictionary<string, string>()
            {
                { nameof(request.Name), request.Name },
                { nameof(request.Role), request.Role ?? "" },
            };

            var claimsJson = JsonSerializer.Serialize(claims);

            var user = new AppUser
            {
                Name = request.Name,
                PasswordHash = Hasher.GetSha256Hash(request.Password),
                Claims = claimsJson
            };

            _dbContext.AppUsers.Add(user);
            await _dbContext.SaveChangesAsync();

            return HandlerResult<UserDto>.Ok(request);
        }
    }
}
