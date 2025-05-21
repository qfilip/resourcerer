using FluentValidation;
using FluentValidation.Results;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Resourcerer.Application.Logic.Handlers;
using Resourcerer.Logic.Models;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.Entities;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.Utilities;

namespace Resourcerer.Logic.V1;

public class CreateUnitOfMeasure
{
    public class Handler : IAppHandler<V1CreateUnitOfMeasure, UnitOfMeasureDto>
    {
        private readonly AppDbContext _appDbContext;
        private readonly Validator _validator;
        private readonly IMapper _mapper;

        public Handler(AppDbContext appDbContext, Validator validator, IMapper mapper)
        {
            _appDbContext = appDbContext;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<HandlerResult<UnitOfMeasureDto>> Handle(V1CreateUnitOfMeasure request)
        {
            var company = await _appDbContext.Companies
                .Include(x => x.UnitsOfMeasure)
                .Where(x => x.Id == request.CompanyId)
                .Select(x => new
                {
                    CompanyId = x.Id,
                    UnitsOfMeasure = x.UnitsOfMeasure
                        .Select(x => new
                        {
                            Name = x.Name,
                            Symbol = x.Symbol
                        })
                })
                .FirstOrDefaultAsync();

            if (company == null)
            {
                return HandlerResult<UnitOfMeasureDto>.NotFound("Company not found");
            }

            var errors = new List<string>();

            if (company.UnitsOfMeasure.Any(x => x.Name == request.Name))
                errors.Add("Unit of measure with the same name already exists");

            if (company.UnitsOfMeasure.Any(x => x.Symbol == request.Symbol))
                errors.Add("Unit of measure with the same symbol already exists");

            if(errors.Count > 0)
                return HandlerResult<UnitOfMeasureDto>.Rejected(errors);

            var entity = new UnitOfMeasure
            {
                Id = Guid.NewGuid(),
                CompanyId = request.CompanyId,
                Name = request.Name,
                Symbol = request.Symbol
            };

            _appDbContext.UnitsOfMeasure.Add(entity);
            await _appDbContext.SaveChangesAsync();

            return HandlerResult<UnitOfMeasureDto>.Ok(_mapper.Map<UnitOfMeasureDto>(entity));
        }

        public ValidationResult Validate(V1CreateUnitOfMeasure request) => _validator.Validate(request);
    }
    public class Validator : AbstractValidator<V1CreateUnitOfMeasure>
    {
        public Validator()
        {
            RuleFor(x => x.CompanyId)
                .NotEmpty().WithMessage("Company id cannot be empty");

            RuleFor(x => x.Name)
                .Must(Validation.UnitOfMeasure.Name)
                .WithMessage(Validation.UnitOfMeasure.NameError);

            RuleFor(x => x.Symbol)
                .Must(Validation.UnitOfMeasure.Symbol)
                .WithMessage(Validation.UnitOfMeasure.SymbolError);
        }
    }
}
