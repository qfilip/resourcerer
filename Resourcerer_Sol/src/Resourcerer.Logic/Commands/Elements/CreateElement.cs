using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos;

namespace Resourcerer.Logic.Commands.Elements;

public static class CreateElement
{
    public class Handler : IAppHandler<CreateElementDto, Unit>
    {
        private readonly AppDbContext _appDbContext;

        public Handler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<HandlerResult<Unit>> Handle(CreateElementDto request)
        {
            var existing = await _appDbContext.Elements
                .FirstOrDefaultAsync(x => x.Name == request.Name);
            
            if(existing != null)
            {
                var error = "Element with the same name already exist";
                return HandlerResult<Unit>.ValidationError(error);
            }

            var category = await _appDbContext.Categories
                .FirstOrDefaultAsync(x => x.Id == request.CategoryId);

            if (category == null)
            {
                var error = "Requested category doesn't exist";
                return HandlerResult<Unit>.ValidationError(error);
            }

            var uom = await _appDbContext.UnitsOfMeasure
                .FirstOrDefaultAsync(x => x.Id == request.UnitOfMeasureId);

            if (uom == null)
            {
                var error = "Requested unit of measure doesn't exist";
                return HandlerResult<Unit>.ValidationError(error);
            }

            var element = new Element
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                CategoryId = request.CategoryId,
                UnitOfMeasureId = request.UnitOfMeasureId
            };

            var price = new Price
            {
                ElementId = element.Id,
                UnitValue = request.UnitPrice
            };

            _appDbContext.Elements.Add(element);
            _appDbContext.Prices.Add(price);

            await _appDbContext.SaveChangesAsync();

            return HandlerResult<Unit>.Ok(new Unit());
        }
    }
}
