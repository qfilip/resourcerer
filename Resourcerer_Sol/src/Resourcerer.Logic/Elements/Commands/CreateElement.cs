using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.Elements;

namespace Resourcerer.Logic.Elements.Commands;

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
            var errors = DtoValidator.Validate<ElementDto, ElementDtoValidator>(request);
            if (errors.Any())
            {
                return HandlerResult<Unit>.ValidationError(errors);
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
