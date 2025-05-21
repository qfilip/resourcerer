using FluentValidation;
using FluentValidation.Results;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Resourcerer.Application.Logic.Handlers;
using Resourcerer.Logic.Models;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.Dtos.Entities;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.Utilities;

namespace Resourcerer.Logic.V1;

public static class EditUnitOfMeasure
{
    public class Handler : IAppHandler<V1EditUnitOfMeasure, UnitOfMeasureDto>
    {
        private readonly AppDbContext _dbContext;
        private readonly Validator _validator;
        private readonly IMapper _mapper;

        public Handler(AppDbContext dbContext, Validator validator, IMapper mapper)
        {
            _dbContext = dbContext;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<HandlerResult<UnitOfMeasureDto>> Handle(V1EditUnitOfMeasure request)
        {
            var entity = await _dbContext.UnitsOfMeasure
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            if(entity == null)
                return HandlerResult<UnitOfMeasureDto>.NotFound();
            
            entity.Name = request.Name;
            entity.Symbol = request.Symbol;

            await _dbContext.SaveChangesAsync();

            return HandlerResult<UnitOfMeasureDto>.Ok(_mapper.Map<UnitOfMeasureDto>(entity));
        }

        public ValidationResult Validate(V1EditUnitOfMeasure request) => _validator.Validate(request);
    }

    public class Validator : AbstractValidator<V1EditUnitOfMeasure>
    {
        public Validator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Unit of Measure id cannot be empty");

            RuleFor(x => x.Name)
                .Must(Validation.UnitOfMeasure.Name)
                .WithMessage(Validation.UnitOfMeasure.NameError);

            RuleFor(x => x.Symbol)
                .Must(Validation.UnitOfMeasure.Symbol)
                .WithMessage(Validation.UnitOfMeasure.SymbolError);
        }
    }
}
