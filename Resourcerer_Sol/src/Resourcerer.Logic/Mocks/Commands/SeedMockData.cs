using MediatR;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Mocks;

namespace Resourcerer.Logic.Mocks.Commands;
public static class SeedMockData
{
    public class Command : IRequest<Unit>
    {
        public Command(DatabaseData dbData)
        {
            DatabaseData = dbData;
        }

        public DatabaseData DatabaseData { get; }
    }

    public class Handler : IRequestHandler<Command, Unit>
    {
        private readonly AppDbContext _dbContext;

        public Handler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            _dbContext.Categories.AddRange(request.DatabaseData.Categories!);
            _dbContext.UnitsOfMeasure.AddRange(request.DatabaseData.UnitOfMeasures!);
            _dbContext.Elements.AddRange(request.DatabaseData.Elements!);
            _dbContext.Composites.AddRange(request.DatabaseData.Composites!);
            _dbContext.CompositeSoldEvents.AddRange(request.DatabaseData.CompositeSoldEvents!);
            _dbContext.Excerpts.AddRange(request.DatabaseData.Excerpts!);
            _dbContext.Prices.AddRange(request.DatabaseData.Prices!);
            _dbContext.ElementPurchasedEvents.AddRange(request.DatabaseData.ElementPurchasedEvents!);

            await _dbContext.SaveChangesAsync();

            return new Unit();
        }
    }
}

