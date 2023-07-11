using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Enums;
using Resourcerer.Dtos;

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

            var category = await _appDbContext.Categories
                .FirstOrDefaultAsync(x => x.Id == request.CategoryId);

            if (category == null)
            {
                var error = "Requested category doesn't exist";
                return HandlerResult<Unit>.ValidationError(error);
            }

            var unitOfMeasure = await _appDbContext.UnitsOfMeasure
                .FirstOrDefaultAsync(x => x.Id == request.UnitOfMeasureId);

            if (unitOfMeasure == null)
            {
                var error = "Requested unit of measure doesn't exist";
                return HandlerResult<Unit>.ValidationError(error);
            }

            var requestElementIds = request.Elements!
                .Select(x => x.ElementId)
                .ToArray();
            
            var requiredElements = await _appDbContext.Items
                .Where(x => requestElementIds.Contains(x.Id))
                .Include(x => x.Prices) // EFCore bug with global query filter
                .ToArrayAsync();
            
            if (requestElementIds.Length != requiredElements.Length)
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

                if(r.Prices.Count(x => x.EntityStatus == eEntityStatus.Active) > 1)
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
                Name = request.Name,
                CategoryId = category.Id,
                UnitOfMeasureId = unitOfMeasure.Id,
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
            _appDbContext.SaveChanges();
            _appDbContext.Excerpts.AddRange(excerpts);
            _appDbContext.SaveChanges();
            _appDbContext.Prices.Add(price);

            await _appDbContext.SaveChangesAsync();

            return HandlerResult<Unit>.Ok(new Unit());
        }
    }
}
