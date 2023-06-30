﻿using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Mocks;

namespace Resourcerer.Logic.Commands.Mocks;
public static class SeedMockData
{
    public class Handler : IRequestHandler<DatabaseData, Unit>
    {
        private readonly AppDbContext _dbContext;

        public Handler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<HandlerResult<Unit>> Handle(DatabaseData dbData)
        {
            _dbContext.Categories.AddRange(dbData.Categories!);
            _dbContext.UnitsOfMeasure.AddRange(dbData.UnitOfMeasures!);
            _dbContext.Excerpts.AddRange(dbData.Excerpts!);
            _dbContext.Prices.AddRange(dbData.Prices!);

            _dbContext.Composites.AddRange(dbData.Composites!);
            _dbContext.CompositeSoldEvents.AddRange(dbData.CompositeSoldEvents!);

            _dbContext.Elements.AddRange(dbData.Elements!);
            _dbContext.ElementSoldEvents.AddRange(dbData.ElementSoldEvents!);
            _dbContext.ElementPurchasedEvents.AddRange(dbData.ElementPurchasedEvents!);

            await _dbContext.SaveChangesAsync();

            return HandlerResult<Unit>.Ok(new Unit());
        }
    }
}
