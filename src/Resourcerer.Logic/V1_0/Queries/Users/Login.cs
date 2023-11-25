using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.Dtos;
using Resourcerer.Utilities.Cryptography;

namespace Resourcerer.Logic.Queries.V1_0;

public static class Login
{
    public class Handler : IAppHandler<AppUserDto, AppUserDto>
    {
        private readonly AppDbContext _appDbContext;
        public Handler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<HandlerResult<AppUserDto>> Handle(AppUserDto request)
        {
            var user = await _appDbContext.AppUsers
                .FirstOrDefaultAsync(x => x.Name == request.Name);

            if (user == null)
            {
                return HandlerResult<AppUserDto>.NotFound($"User with name {request.Name} not found");
            }

            var hash = Hasher.GetSha256Hash(request.Password!);

            if (user.PasswordHash != hash)
            {
                return HandlerResult<AppUserDto>.Rejected("Bad credentials");
            }

            var dto = new AppUserDto
            {
                Name = user.Name,
                Permissions = Permissions.GetPermissionDictFromString(user.Permissions!)
            };

            return HandlerResult<AppUserDto>.Ok(dto);
        }
    }
}

