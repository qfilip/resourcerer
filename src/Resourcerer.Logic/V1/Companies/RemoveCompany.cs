using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Resourcerer.Application.Abstractions.Handlers;
using Resourcerer.Application.Auth.Abstractions;
using Resourcerer.Application.Models;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Enums;
using Resourcerer.Dtos.V1;

namespace Resourcerer.Logic.V1;

public class RemoveCompany
{
    public class Handler : IAppHandler<V1RemoveCompany, Unit>
    {
        private readonly AppDbContext _dbContext;
        private readonly Validator _validatior;
        private readonly IAppIdentityService<AppUser> _identityService;

        public Handler(AppDbContext dbContext, Validator validatior, IAppIdentityService<AppUser> identityService)
        {
            _dbContext = dbContext;
            _validatior = validatior;
            _identityService = identityService;
        }

        public async Task<HandlerResult<Unit>> Handle(V1RemoveCompany request)
        {
            var userHasPermissions =
                _identityService.Get().IsAdmin &&
                _identityService.Get().CompanyId == request.CompanyId;

            if (!userHasPermissions)
                return HandlerResult<Unit>.Rejected("Insufficient permissions to perform the operation");

            var company = await _dbContext.Companies
                .FirstOrDefaultAsync(x => x.Id == request.CompanyId);

            if (company == null)
                return HandlerResult<Unit>.NotFound();

            company.EntityStatus = eEntityStatus.Deleted;

            await _dbContext.SaveChangesAsync();

            return HandlerResult<Unit>.Ok(Unit.New);
        }

        public ValidationResult Validate(V1RemoveCompany request) => _validatior.Validate(request);
    }

    public class Validator : AbstractValidator<V1RemoveCompany>
    {
        public Validator()
        {
            RuleFor(x => x.CompanyId)
                .NotEmpty().WithMessage("Company Id name cannot be empty");
        }
    }
}
