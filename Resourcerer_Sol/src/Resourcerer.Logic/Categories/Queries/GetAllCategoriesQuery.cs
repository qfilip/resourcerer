using MediatR;
using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.Dtos.Categories;

namespace Resourcerer.Logic.Categories.Queries;

public static class GetAllCategories
{
    public class Query : IRequest<List<CategoryDto>> {}

    public class Handler : IRequestHandler<Query, List<CategoryDto>>
    {
        private readonly AppDbContext _appDbContext;

        public Handler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<List<CategoryDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            return await _appDbContext.Categories.Select(x => new CategoryDto {
                Id = x.Id,
                Name = x.Name,
                ParentCategoryId = x.ParentCategoryId,
                CreatedAt = x.CreatedAt,
                ModifiedAt = x.ModifiedAt
            }).ToListAsync();
        }
    }
}
