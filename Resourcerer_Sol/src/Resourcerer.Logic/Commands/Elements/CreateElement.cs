using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.Elements;

namespace Resourcerer.Logic.Commands.Elements;

public static class CreateElement
{
    public class Handler : IRequestHandler<ElementDto, Unit>
    {
        private readonly IAppDbContext _appDbContext;

        public Handler(IAppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<HandlerResult<Unit>> Handle(ElementDto request)
        {
            var category = await _appDbContext.Categories
                .FirstOrDefaultAsync(x => x.Id == request.CategoryId);

            if (category == null)
            {
                HandlerResult<Unit>.ValidationError("Cannot find supplied category");
            }

            var unitOfMeasure = await _appDbContext.UnitsOfMeasure
                .FirstOrDefaultAsync(x => x.Id == request.UnitOfMeasureId);

            if (unitOfMeasure == null)
            {
                HandlerResult<Unit>.ValidationError("Cannot find supplied unit of measure");
            }

            var entity = new Element
            {
                Name = request.Name,
                CategoryId = request.CategoryId,
                UnitOfMeasureId = request.UnitOfMeasureId
            };

            _appDbContext.Elements.Add(entity);
            await _appDbContext.SaveChangesAsync();

            return HandlerResult<Unit>.Ok(new Unit());
        }
    }
}
