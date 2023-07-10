using Resourcerer.DataAccess.Contexts;
using Resourcerer.Dtos;

namespace Resourcerer.Logic.Commands.ElementEvents;

public static class CreateElementOrderedEvent
{
    public class Handler : IAppHandler<InstanceOrderedEventDto, Unit>
    {
        private readonly AppDbContext _appDbContext;
        public Handler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task<HandlerResult<Unit>> Handle(InstanceOrderedEventDto request)
        {
            //var element = await _appDbContext.Elements
            //    .Include(x => x.UnitOfMeasure)
            //    .FirstOrDefaultAsync(x => x.Id == request.ElementId);

            //if (element == null)
            //{
            //    return HandlerResult<Unit>
            //        .ValidationError($"Element with id: {request.ElementId} not found");
            //}

            //var entity = new ElementPurchasedEvent
            //{
            //    ElementId = element.Id,
            //    UnitOfMeasure = element.UnitOfMeasure,
            //    UnitPrice = request.UnitPrice,
            //    UnitsBought = request.UnitsBought,
            //    ExpectedDeliveryTime = request.ExpectedDeliveryTime,
            //    TotalDiscountPercent = request.TotalDiscountPercent
            //};

            //_appDbContext.ElementPurchasedEvents.Add(entity);
            //await _appDbContext.SaveChangesAsync();

            return HandlerResult<Unit>.Ok(new Unit());
        }
    }
}
