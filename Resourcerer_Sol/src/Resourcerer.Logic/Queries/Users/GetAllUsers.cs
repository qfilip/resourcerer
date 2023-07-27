using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.Dtos;

namespace Resourcerer.Logic.Queries.Users;

public class GetAllUsers
{
    public class Handler : IAppHandler<Unit, AppUserDto[]>
    {
        private readonly AppDbContext _appDbContext;
        public Handler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<HandlerResult<AppUserDto[]>> Handle(Unit _)
        {
            var users = await _appDbContext.AppUsers.ToArrayAsync();

            var dtos = users.Select(x => new AppUserDto
            {
                Id = x.Id,
                Name = x.Name,
                Permissions = Permissions.GetPermissionDictFromString(x.Permissions!)
            })
            .ToArray();

            return HandlerResult<AppUserDto[]>.Ok(dtos);
        }
    }
}
