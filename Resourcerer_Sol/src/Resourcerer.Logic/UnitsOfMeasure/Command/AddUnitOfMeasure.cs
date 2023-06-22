using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.UnitsOfMeasure;

namespace Resourcerer.Logic.UnitsOfMeasure.Command;

public class AddUnitOfMeasure
{
    public class Handler : IRequestHandler<UnitOfMeasureDto, Unit>
    {
        private readonly AppDbContext _appDbContext;

        public Handler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<HandlerResult<Unit>> Handle(UnitOfMeasureDto request)
        {
            var errors = DtoValidator.Validate<UnitOfMeasureDto, UnitOfMeasureDtoValidator>(request);
            if (errors.Any())
            {
                return HandlerResult<Unit>.ValidationError(errors);
            }

            var entity = new UnitOfMeasure
            {
                Name = request.Name,
                Symbol = request.Symbol
            };

            _appDbContext.UnitsOfMeasure.Add(entity);
            await _appDbContext.SaveChangesAsync();

            return HandlerResult<Unit>.Ok(new Unit());
        }
    }
}
