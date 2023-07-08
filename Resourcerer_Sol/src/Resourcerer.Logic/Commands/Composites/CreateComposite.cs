using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Resourcerer.Logic.Commands.Composites;

public static class CreateComposite
{
    public class Handler : IAppHandler<CreateCompositeDto, Unit>
    {
        private readonly AppDbContext _appDbContext;
        private readonly ILogger<Handler> _logger;

        public Handler(AppDbContext appDbContext, ILogger<Handler> logger)
        {
            _appDbContext = appDbContext;
            _logger = logger;
        }

        public async Task<HandlerResult<Unit>> Handle(CreateCompositeDto request)
        {
            var compositeExists = await _appDbContext.Composites
                .CountAsync(x => x.Name == request.Name) > 0;

            if(compositeExists)
            {
                var error = "Composite with the same name already exists";
                return HandlerResult<Unit>.ValidationError(error);
            }

            var categoryExists = await _appDbContext.Categories
                .CountAsync(x => x.Id == request.CategoryId) > 0;

            if (!categoryExists)
            {
                var error = "Requested category doesn't exist";
                return HandlerResult<Unit>.ValidationError(error);
            }

            var requestElementIds = request.Elements!
                .Select(x => x.ElementId)
                .ToArray();
            
            var requiredElements = await _appDbContext.Elements
                .Where(x => requestElementIds.Contains(x.Id))
                .Include(x => x.Prices)
                .ToArrayAsync();

            if(requestElementIds.Length != requiredElements.Length)
            {
                var error = "Not all required elements found";
                return HandlerResult<Unit>.ValidationError(error);
            }

            var elementErrors = new List<string>();
            foreach (var r in requiredElements)
            {
                if(!r.Prices.Any())
                {
                    elementErrors.Add($"Element {r.Id} doesn't have a price");
                }

                if(r.Prices.Count > 0)
                {
                    elementErrors.Add($"Element {r.Id} has more than one active price");
                    _logger.LogWarning("Element {id} has more than one active price", r.Id);
                }
            }

            if(elementErrors.Any())
            {
                return HandlerResult<Unit>.ValidationError(elementErrors.ToArray());
            }

            var composite = new Composite
            {
                Id = Guid.NewGuid(),
                CategoryId = request.CategoryId,
                Name = request.Name
            };

            var excerpts = request.Elements!
                .Select(x => new Excerpt
                {
                    CompositeId = composite.Id,
                    ElementId = x.ElementId,
                    Quantity = x.Quantity
                });

            var price = new Price
            {
                CompositeId = composite.Id,
                UnitValue = request.UnitPrice
            };

            _appDbContext.Composites.Add(composite);
            _appDbContext.Excerpts.AddRange(excerpts);
            _appDbContext.Prices.Add(price);

            await _appDbContext.SaveChangesAsync();

            return HandlerResult<Unit>.Ok(new Unit());
        }
    }
}
