using FluentValidation;
using FluentValidation.Results;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.V1;

namespace Resourcerer.Logic.Commands.V1_0;

public class CreateUnitOfMeasure
{
    public class Handler : IAppHandler<V1CreateUnitOfMeasure, Unit>
    {
        private readonly AppDbContext _appDbContext;

        public Handler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<HandlerResult<Unit>> Handle(V1CreateUnitOfMeasure request)
        {
            var companyExists = _appDbContext.Companies
                .Where(x => x.Id == request.CompanyId)
                .Count() > 0;

            if(!companyExists)
            {
                return HandlerResult<Unit>.Rejected("Company id not found");
            }

            var entity = new UnitOfMeasure
            {
                CompanyId = request.CompanyId,
                Name = request.Name,
                Symbol = request.Symbol
            };

            _appDbContext.UnitsOfMeasure.Add(entity);
            await _appDbContext.SaveChangesAsync();

            return HandlerResult<Unit>.Ok(new Unit());
        }

        public ValidationResult Validate(V1CreateUnitOfMeasure request) => new Validator().Validate(request);

        private class Validator : AbstractValidator<V1CreateUnitOfMeasure>
        {
            public Validator()
            {
                RuleFor(x => x.CompanyId)
                    .NotEmpty().WithMessage("Company id cannot be empty");

                RuleFor(x => x.Name)
                    .NotEmpty().WithMessage("Unit of measure name cannot be empty")
                    .Length(min: 2, max: 50).WithMessage("Unit of measure name must be between 2 and 50 characters long");

                RuleFor(x => x.Symbol)
                    .NotEmpty().WithMessage("Unit of measure symbol cannot be empty")
                    .Length(min: 1, max: 12).WithMessage("Unit of measure symbol must be between 1 and 12 characters long");
            }
        }
    }
}
