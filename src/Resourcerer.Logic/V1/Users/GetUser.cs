using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Resourcerer.Application.Logic.Handlers;
using Resourcerer.Logic.Models;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.Dtos.Entities;
using Resourcerer.Logic.Utilities.Query;

namespace Resourcerer.Logic.V1;

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
                .Where(x => x.Id == request)
                .Select(AppUsers.DefaultDtoProjection)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return HandlerResult<AppUserDto>.NotFound($"User with id {request} doesn't exist");
            }

            return HandlerResult<AppUserDto>.Ok(user);
        }

        public ValidationResult Validate(Guid request) => new ValidationResult();
    }
}
