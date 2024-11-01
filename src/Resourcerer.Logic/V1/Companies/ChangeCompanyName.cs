using FluentValidation;
using FluentValidation.Results;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Resourcerer.Application.Abstractions.Handlers;
using Resourcerer.Application.Models;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.Dtos.Entity;
using Resourcerer.Dtos.V1;
using Resourcerer.Identity.Abstractions;
using Resourcerer.Identity.Models;

namespace Resourcerer.Logic.V1;

public class ChangeCompanyName
{
    public class Handler : IAppHandler<V1ChangeCompanyName, CompanyDto>
    {
        private readonly AppDbContext _dbContext;
        private readonly Validator _validatior;
        private readonly IAppIdentityService<AppIdentity> _identityService;
        private readonly IMapper _mapper;

        public Handler(
            AppDbContext dbContext,
            Validator validatior,
            IAppIdentityService<AppIdentity> identityService,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _validatior = validatior;
            _identityService = identityService;
            _mapper = mapper;
        }

        public async Task<HandlerResult<CompanyDto>> Handle(V1ChangeCompanyName request)
        {
            var userHasPermissions =
                _identityService.Get().Admin &&
                _identityService.Get().CompanyId == request.CompanyId;

            if (!userHasPermissions)
                return HandlerResult<CompanyDto>.Rejected("Only admin users can perform this change");

            var entity = await _dbContext.Companies
                .FirstOrDefaultAsync(x => x.Id == request.CompanyId);

            if(entity == null)
                return HandlerResult<CompanyDto>.NotFound();

            entity.Name = request.NewName;

            await _dbContext.SaveChangesAsync();

            return HandlerResult<CompanyDto>.Ok(_mapper.Map<CompanyDto>(entity));
        }

        public ValidationResult Validate(V1ChangeCompanyName request) => _validatior.Validate(request);
    }

    public class Validator : AbstractValidator<V1ChangeCompanyName>
    {
        public Validator()
        {
            RuleFor(x => x.CompanyId)
                .NotEmpty().WithMessage("Company Id name cannot be empty");
            
            RuleFor(x => x.NewName)
                .NotEmpty().WithMessage("Category name cannot be empty")
                .Length(min: 3, max: 50).WithMessage("Category name must be between 3 and 50 characters long");
        }
    }
}
