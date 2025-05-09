﻿using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Resourcerer.Application.Logic.Handlers;
using Resourcerer.Logic.Models;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Enums;
using Resourcerer.Dtos.V1;
using Resourcerer.Identity.Abstractions;
using Resourcerer.Identity.Models;

namespace Resourcerer.Logic.V1;

public class RemoveCompany
{
    public class Handler : IAppHandler<V1RemoveCompany, Unit>
    {
        private readonly AppDbContext _dbContext;
        private readonly Validator _validatior;
        private readonly IAppIdentityService<AppIdentity> _identityService;

        public Handler(AppDbContext dbContext, Validator validatior, IAppIdentityService<AppIdentity> identityService)
        {
            _dbContext = dbContext;
            _validatior = validatior;
            _identityService = identityService;
        }

        public async Task<HandlerResult<Unit>> Handle(V1RemoveCompany request)
        {
            var userHasPermissions =
                _identityService.Get().Admin &&
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
