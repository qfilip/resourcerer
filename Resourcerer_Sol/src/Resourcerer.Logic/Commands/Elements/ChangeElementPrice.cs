using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Enums;
using Resourcerer.Dtos;

namespace Resourcerer.Logic.Commands.Elements;

public class ChangeElementPrice
{
    public class Handler : IAppHandler<ChangePriceDto, Unit>
    {
        private readonly AppDbContext _appDbContext;
        private readonly ILogger<Handler> _logger;

        public Handler(AppDbContext appDbContext, ILogger<Handler> logger)
        {
            _appDbContext = appDbContext;
            _logger = logger;
        }

        public async Task<HandlerResult<Unit>> Handle(ChangePriceDto request)
        {   
            var element = await _appDbContext.Elements
                .Where(x => x.Id == request.EntityId)
                .Include(x => x.Prices)
                .FirstOrDefaultAsync();

            if(element == null)
            {
                return HandlerResult<Unit>.ValidationError($"Element with id {request.EntityId} doesn't exist");
            }

            if(element.Prices.Count(x => x.EntityStatus == eEntityStatus.Active) > 1)
            {
                _logger.LogWarning("Element with id {id} had multiple active prices", element.Id);
            }

            foreach (var p in element.Prices)
                p.EntityStatus = eEntityStatus.Deleted;

            var price = new Price
            {
                ElementId = request.EntityId,
                UnitValue = request.UnitPrice
            };

            _appDbContext.Prices.Add(price);
            await _appDbContext.SaveChangesAsync();

            return HandlerResult<Unit>.Ok(new Unit());
        }
    }
}
