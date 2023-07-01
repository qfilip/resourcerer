using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.UnitsOfMeasure;

namespace Resourcerer.Logic.Commands.UnitsOfMeasure;

public class CreateUnitOfMeasure
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
