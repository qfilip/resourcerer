using FluentValidation.Results;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.Dtos.Fake;

namespace Resourcerer.Logic.Fake;

public static class Seed
{
    public class Handler : IAppHandler<DataSeedDto, Unit>
    {
        private readonly AppDbContext _dbContext;

        public Handler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<HandlerResult<Unit>> Handle(DataSeedDto request)
        {
            _dbContext.Companies.AddRange(request.Companies);
            _dbContext.AppUsers.AddRange(request.AppUsers);
            _dbContext.Categories.AddRange(request.Categories);
            _dbContext.UnitsOfMeasure.AddRange(request.UnitsOfMeasure);
            _dbContext.Excerpts.AddRange(request.Excerpts);
            _dbContext.Prices.AddRange(request.Prices);
            _dbContext.Items.AddRange(request.Items);
            _dbContext.ItemProductionOrders.AddRange(request.ItemProductionOrders);
            _dbContext.Instances.AddRange(request.Instances);
            _dbContext.InstanceOrderedEvents.AddRange(request.InstanceOrderedEvents);
            _dbContext.InstanceReservedEvents.AddRange(request.InstanceReservedEvents);
            _dbContext.InstanceDiscardedEvents.AddRange(request.InstanceDiscardedEvents);

            await _dbContext.SaveChangesAsync();

            return HandlerResult<Unit>.Ok(Unit.New);
        }

        public ValidationResult Validate(DataSeedDto request) => new ValidationResult();
    }
}
