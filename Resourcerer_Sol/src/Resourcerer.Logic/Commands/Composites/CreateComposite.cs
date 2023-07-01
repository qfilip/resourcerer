using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.Composites;

namespace Resourcerer.Logic.Commands.Composites;

public static class CreateComposite
{
    public class Handler : IRequestHandler<CreateCompositeDto, Unit>
    {
        private readonly IAppDbContext _appDbContext;

        public Handler(IAppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<HandlerResult<Unit>> Handle(CreateCompositeDto request)
        {
            var category = await _appDbContext.Categories
                .FirstOrDefaultAsync(x => x.Id == request.CategoryId);

            if(category == null)
            {
                return HandlerResult<Unit>.ValidationError("Cannot find supplied category");
            }

            var elementIds = request.Elements!.Select(x => x.ElementId).ToArray();

            var elementCount = await _appDbContext.Elements
                .Where(x => elementIds.Contains(x.Id))
                .CountAsync();

            if(elementIds.Length != elementCount)
            {
                return HandlerResult<Unit>.ValidationError("Not all required elements found");
            }

            var composite = new Composite
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                CategoryId = request.CategoryId,
            };

            var price = new Price
            {
                CompositeId = composite.Id,
                UnitValue = request.PriceByUnit
            };

            var excerpts = request.Elements!
                .Select(x => new Excerpt
                {
                    CompositeId = composite.Id,
                    ElementId = x.ElementId,
                    Quantity = x.Quantity
                });

            _appDbContext.Composites.Add(composite);
            _appDbContext.Prices.Add(price);
            _appDbContext.Excerpts.AddRange(excerpts);

            await _appDbContext.SaveChangesAsync();

            return HandlerResult<Unit>.Ok(new Unit());
        }
    }
}
