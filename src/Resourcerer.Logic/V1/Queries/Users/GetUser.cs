using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.Dtos;
using Resourcerer.Dtos.Entity;

namespace Resourcerer.Logic.Queries.V1;

public static class GetUser
{
    public class Handler : IAppHandler<Guid, AppUserDto>
    {
        private readonly AppDbContext _dbContext;
        public Handler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<HandlerResult<AppUserDto>> Handle(Guid request)
        {
            var user = await _dbContext.AppUsers
                .FirstOrDefaultAsync(x => x.Id == request);

            if(user == null)
            {
                return HandlerResult<AppUserDto>.NotFound($"User with id {request} doesn't exist");
            }
            
            var dto = new AppUserDto()
            {
                Id = user.Id,
                Name = user.Name,
                Permissions = Permissions.GetPermissionDictFromString(user.Permissions!)
            };
                
            return HandlerResult<AppUserDto>.Ok(dto);
        }

        public ValidationResult Validate(Guid request) => new ValidationResult();
    }
}
