using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Mocks;

namespace Resourcerer.Logic.Commands.Mocks;
public static class SeedMockData
{
    public class Handler : IAppHandler<DatabaseData, Unit>
    {
        private readonly AppDbContext _dbContext;

        public Handler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<HandlerResult<Unit>> Handle(DatabaseData dbData)
        {
            _dbContext.AppUsers.AddRange(dbData.AppUsers!);
            _dbContext.Categories.AddRange(dbData.Categories!);
            _dbContext.UnitsOfMeasure.AddRange(dbData.UnitsOfMeasure!);
            _dbContext.Excerpts.AddRange(dbData.Excerpts!);
            _dbContext.Prices.AddRange(dbData.Prices!);
            _dbContext.Items.AddRange(dbData.Elements!);

            await _dbContext.BaseSaveChangesAsync();

            return HandlerResult<Unit>.Ok(new Unit());
        }
    }
}

