using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.Elements;

namespace Resourcerer.Logic.Commands.Elements;

public static class CreateElement
{
    public class Handler : IRequestHandler<CreateElementDto, Unit>
    {
        private readonly IAppDbContext _appDbContext;

        public Handler(IAppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<HandlerResult<Unit>> Handle(CreateElementDto request)
        {
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
