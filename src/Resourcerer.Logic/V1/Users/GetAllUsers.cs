using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.Dtos;
using Resourcerer.Dtos.Entity;

namespace Resourcerer.Logic.V1.Users;

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
            var users = await _appDbContext.AppUsers
                .IgnoreQueryFilters()
                .ToArrayAsync();

            var dtos = users.Select(x => new AppUserDto
            {
                Id = x.Id,
                Name = x.Name,
                PermissionsMap = Permissions.GetPermissionsMap(x.Permissions!),
                EntityStatus = x.EntityStatus
            })
            .ToArray();

            return HandlerResult<AppUserDto[]>.Ok(dtos);
        }

        public ValidationResult Validate(Unit request) => new ValidationResult();
    }
}
