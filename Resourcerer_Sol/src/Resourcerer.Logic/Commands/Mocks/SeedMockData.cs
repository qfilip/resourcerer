using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Mocks;

namespace Resourcerer.Logic.Commands.Mocks;
public static class SeedMockData
{
    public class Handler : IRequestHandler<DatabaseData, Unit>
    {
        private readonly IAppDbContext _dbContext;

        public Handler(IAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<HandlerResult<Unit>> Handle(DatabaseData dbData)
        {
            _dbContext.AppUsers.AddRange(dbData.AppUsers!);
            _dbContext.Categories.AddRange(dbData.Categories!);
            _dbContext.UnitsOfMeasure.AddRange(dbData.UnitOfMeasures!);
            _dbContext.Excerpts.AddRange(dbData.Excerpts!);
            _dbContext.Prices.AddRange(dbData.OldPrices!);

            _dbContext.Composites.AddRange(dbData.Composites!);
            _dbContext.CompositeSoldEvents.AddRange(dbData.CompositeSoldEvents!);

            _dbContext.Elements.AddRange(dbData.Elements!);
            _dbContext.ElementSoldEvents.AddRange(dbData.ElementSoldEvents!);
            _dbContext.ElementPurchasedEvents.AddRange(dbData.ElementPurchasedEvents!);

            await _dbContext.BaseSaveChangesAsync();

            return HandlerResult<Unit>.Ok(new Unit());
        }
    }
}

