using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos;
using Resourcerer.Utilities.Cryptography;
using System.Text.Json;

namespace Resourcerer.Logic.Commands.Users;
public static class Register
{
    public class Handler : IAppHandler<AppUserDto, string>
    {
        private readonly IAppDbContext _appDbContext;
        public Handler(IAppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<HandlerResult<string>> Handle(AppUserDto request)
        {
            var entity = new AppUser
            {
                Name = request.Name,
                PasswordHash = Hasher.GetSha256Hash(request.Password!),
                Permissions = JsonSerializer.Serialize(new Dictionary<string, string>())
            };

            _appDbContext.AppUsers.Add(entity);
            await _appDbContext.SaveChangesAsync();

            var message = "Account created. Contact administrator to get permissions.";
            return HandlerResult<string>.Ok(message);
        }
    }
}
