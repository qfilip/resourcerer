using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.Composites;

namespace Resourcerer.Logic.Commands.Composites;

public static class CreateComposite
{
    public class Handler : IRequestHandler<CompositeDto, Unit>
    {
        private readonly IAppDbContext _appDbContext;

        public Handler(IAppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<HandlerResult<Unit>> Handle(CompositeDto request)
        {
            var category = await _appDbContext.Categories
                .FirstOrDefaultAsync(x => x.Id == request.CategoryId);

            if(category == null)
            {
                HandlerResult<Unit>.ValidationError("Cannot find supplied category");
            }

            var entity = new Composite
            {
                Name = request.Name,
                CategoryId = request.CategoryId,
            };

            _appDbContext.Composites.Add(entity);
            await _appDbContext.SaveChangesAsync();

            return HandlerResult<Unit>.Ok(new Unit());
        }
    }
}
