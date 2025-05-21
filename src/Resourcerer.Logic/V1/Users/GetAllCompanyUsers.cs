using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Resourcerer.Application.Logic.Handlers;
using Resourcerer.Logic.Models;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.Dtos.Entities;
using Resourcerer.Logic.Utilities.Query;

namespace Resourcerer.Logic.V1;

public static class GetAllCompanyUsers
{
    public class Handler : IAppHandler<Guid, AppUserDto[]>
    {
        private readonly AppDbContext _dbContext;
        private readonly Validator _validator;
        public Handler(AppDbContext dbContext, Validator validator)
        {
            _dbContext = dbContext;
            _validator = validator;
        }

        public async Task<HandlerResult<AppUserDto[]>> Handle(Guid request)
        {
            var users = await _dbContext.AppUsers
                .Where(x => x.CompanyId == request)
                .Select(AppUsers.DefaultDtoProjection)
                .ToArrayAsync();

            return HandlerResult<AppUserDto[]>.Ok(users);
        }

        public ValidationResult Validate(Guid request) => _validator.Validate(request);
    }

    public class Validator : AbstractValidator<Guid>
    {
        public Validator()
        {
            RuleFor(x => x)
                .NotEmpty()
                .WithMessage("Company id cannot be empty");
        }
    }
}
