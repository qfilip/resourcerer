using FluentValidation;
using FluentValidation.Results;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos;

namespace Resourcerer.Logic.Commands.V1_0;

public class CreateUnitOfMeasure
{
    public class Handler : IAppHandler<UnitOfMeasureDto, Unit>
    {
        private readonly AppDbContext _appDbContext;

        public Handler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<HandlerResult<Unit>> Handle(UnitOfMeasureDto request)
        {
            var entity = new UnitOfMeasure
            {
                Name = request.Name,
                Symbol = request.Symbol
            };

            _appDbContext.UnitsOfMeasure.Add(entity);
            await _appDbContext.SaveChangesAsync();

            return HandlerResult<Unit>.Ok(new Unit());
        }

        public ValidationResult Validate(UnitOfMeasureDto request) => new Validator().Validate(request);

        private class Validator : AbstractValidator<UnitOfMeasureDto>
        {
            public Validator()
            {
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
