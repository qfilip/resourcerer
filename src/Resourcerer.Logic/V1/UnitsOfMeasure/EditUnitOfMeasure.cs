using FluentValidation;
using FluentValidation.Results;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Resourcerer.Application.Abstractions.Handlers;
using Resourcerer.Application.Models;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.Dtos.Entity;
using Resourcerer.Dtos.V1;

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
                .NotEmpty().WithMessage("Unit of measure name cannot be empty")
                .Length(min: 2, max: 50).WithMessage("Unit of measure name must be between 2 and 50 characters long");

            RuleFor(x => x.Symbol)
                .NotEmpty().WithMessage("Unit of measure symbol cannot be empty")
                .Length(min: 1, max: 12).WithMessage("Unit of measure symbol must be between 1 and 12 characters long");
        }
    }
}
