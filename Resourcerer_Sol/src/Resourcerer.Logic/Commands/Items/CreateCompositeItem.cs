using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos;

namespace Resourcerer.Logic.Commands.Items;

public static class CreateCompositeItem
{
    public class Handler : IAppHandler<CreateCompositeItemDto, Unit>
    {
        private readonly AppDbContext _appDbContext;

        public Handler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<HandlerResult<Unit>> Handle(CreateCompositeItemDto request)
        {
            var existing = await _appDbContext.Items
                .FirstOrDefaultAsync(x => x.Name == request.Name);

            if (existing != null)
            {
                var error = "Element with the same name already exist";
                return HandlerResult<Unit>.Rejected(error);
            }

            var category = await _appDbContext.Categories
                .FirstOrDefaultAsync(x => x.Id == request.CategoryId);

            if (category == null)
            {
                var error = "Requested category doesn't exist";
                return HandlerResult<Unit>.Rejected(error);
            }

            var uom = await _appDbContext.UnitsOfMeasure
                .FirstOrDefaultAsync(x => x.Id == request.UnitOfMeasureId);

            if (uom == null)
            {
                var error = "Requested unit of measure doesn't exist";
                return HandlerResult<Unit>.Rejected(error);
            }

            var requiredElementIds = request.ExcerptMap!.Select(x => x.Key).ToArray();
            
            var requiredItems = await _appDbContext.Items
                .Where(x => requiredElementIds.Contains(x.Id))
                .ToArrayAsync();

            if(requiredItems.Length != requiredElementIds.Length)
            {
                var error = "All required elements don't exist";
                return HandlerResult<Unit>.Rejected(error);
            }

            var composite = new Item
            {
                Id = Guid.NewGuid(),
                
                Name = request.Name,
                PreparationTimeSeconds = request.PreparationTimeSeconds,
                ExpirationTimeSeconds = request.ExpirationTimeSeconds,
                
                CategoryId = category.Id,
                UnitOfMeasureId = uom.Id,
            };

            var price = new Price
            {
                ItemId = composite.Id,
                UnitValue = request.UnitPrice
            };

            var excerpts = request.ExcerptMap!
                .Select(x => new Excerpt
                {
                    CompositeId = composite.Id,
                    ElementId = x.Key,
                    Quantity = x.Value
                }).ToArray();

            _appDbContext.Items.Add(composite);
            _appDbContext.Prices.Add(price);
            _appDbContext.Excerpts.AddRange(excerpts);

            await _appDbContext.SaveChangesAsync();

            return HandlerResult<Unit>.Ok(new Unit());
        }
    }
}
