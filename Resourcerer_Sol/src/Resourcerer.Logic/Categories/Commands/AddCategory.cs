using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.Categories;

namespace Resourcerer.Logic.Categories.Commands;

public class AddCategory
{
    public class Handler : IRequestHandler<CategoryDto, Unit>
    {
        private readonly IAppDbContext _appDbContext;

        public Handler(IAppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<HandlerResult<Unit>> Handle(CategoryDto request)
        {
            var errors = DtoValidator.Validate<CategoryDto, CategoryDtoValidator>(request);
            if(errors.Any())
            {
                return HandlerResult<Unit>.ValidationError(errors);
            }

            var entity = new Category
            {
                Name = request.Name,
                ParentCategoryId = request.ParentCategoryId,
            };

            _appDbContext.Categories.Add(entity);
            await _appDbContext.SaveChangesAsync();

            return HandlerResult<Unit>.Ok(new Unit());
        }
    }
}
