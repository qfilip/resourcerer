using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.Dtos;
using Resourcerer.Dtos.Users;
using System.Text.Json;

namespace Resourcerer.Logic.Commands.Users;

public static class SetPermissions
{
    public class Handler : IRequestHandler<SetUserPermissionsDto, Unit>
    {
        private readonly IAppDbContext _appDbContext;

        public Handler(IAppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<HandlerResult<Unit>> Handle(SetUserPermissionsDto request)
        {
            var errors = Permission.Validate(request.Permissions!);
            if(errors.Any())
            {
                return HandlerResult<Unit>.ValidationError(errors.ToArray());
            }

            var user = await _appDbContext.AppUsers
                .FirstOrDefaultAsync(x => x.Id == request.UserId);

            if(user == null)
            {
                return HandlerResult<Unit>.NotFound($"User with Id {request.UserId} not found");
            }

            user.Permissions = JsonSerializer.Serialize(request.Permissions);

            await _appDbContext.SaveChangesAsync();

            return HandlerResult<Unit>.Ok(new Unit());
        }
    }
}
