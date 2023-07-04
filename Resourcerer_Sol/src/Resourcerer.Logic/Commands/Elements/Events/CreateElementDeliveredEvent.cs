using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos;

namespace Resourcerer.Logic.Commands.Elements.Events;

public class CreateElementDeliveredEvent
{
    public class Handler : IRequestHandler<CreateElementDeliveredEventDto, Unit>
    {
        private readonly IAppDbContext _appDbContext;
        public Handler(IAppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<HandlerResult<Unit>> Handle(CreateElementDeliveredEventDto request)
        {
            var purchaseEvent = await _appDbContext.ElementPurchasedEvents
                .FirstOrDefaultAsync(x => x.Id == request.ElementPurchasedEventId);

            if (purchaseEvent == null)
            {
                return HandlerResult<Unit>.NotFound("Purchase event not found");
            }

            var deliveredEvent = new ElementDeliveredEvent
            {
                ElementPurchasedEventId = request.ElementPurchasedEventId
            };

            var elementInstance = new Instance
            {
                ElementId = purchaseEvent.ElementId,
                Quantity = purchaseEvent.UnitsBought,
                ExpiryDate = request.InstanceExpiryDate
            };

            _appDbContext.ElementDeliveredEvents.Add(deliveredEvent);
            _appDbContext.Instances.Add(elementInstance);

            await _appDbContext.SaveChangesAsync();

            return HandlerResult<Unit>.Ok(new Unit());
        }
    }
}
