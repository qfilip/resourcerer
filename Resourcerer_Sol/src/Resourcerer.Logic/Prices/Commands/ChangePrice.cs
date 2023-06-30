using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.Prices;

namespace Resourcerer.Logic.Prices.Commands;

public static class ChangePrice
{
    public class Handler : IRequestHandler<PriceDto, Unit>
    {
        private readonly IAppDbContext _appDbContext;
        public Handler(IAppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<HandlerResult<Unit>> Handle(PriceDto request)
        {
            var errors = DtoValidator.Validate<PriceDto, PriceDtoValidator>(request);
            if (errors.Any())
            {
                return HandlerResult<Unit>.ValidationError(errors);
            }

            var entity = new Price
            {
                CompositeId = request.CompositeId,
                Value = request.Value
            };

            _appDbContext.Prices.Add(entity);

            await _appDbContext.SaveChangesAsync();

            return HandlerResult<Unit>.Ok(new Unit());
        }
    }
}
