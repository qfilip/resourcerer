using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
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
            if(request.ElementId == null)
            {
                return HandlerResult<Unit>
                    .ValidationError($"ElementId cannot be null");
            }

            var element = await _appDbContext.Elements
                .Include(x => x.UnitOfMeasure)
                .FirstOrDefaultAsync(x => x.Id == request.ElementId);

            if (element == null)
            {
                return HandlerResult<Unit>
                    .ValidationError($"Element with id {request.ElementId} not found");
            }

            var instance = new Instance
            {
                ElementId = element.Id,
                ExpiryDate = request.ExpiryDate
            };

            var entity = new InstanceOrderedEvent
            {
                Instance = instance,
                UnitPrice = request.UnitPrice,
                UnitsOrdered = request.UnitsOrdered,
                TotalDiscountPercent = request.TotalDiscountPercent,
                ExpectedDeliveryTime = request.ExpectedDeliveryTime
            };

            _appDbContext.InstanceOrderedEvents.Add(entity);
            await _appDbContext.SaveChangesAsync();

            return HandlerResult<Unit>.Ok(new Unit());
        }
    }
}
