﻿using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.Dtos;

namespace Resourcerer.Logic.Queries.Categories;

public static class GetAllCategories
{
    public class Handler : IRequestHandler<Unit, List<CategoryDto>>
    {
        private readonly IAppDbContext _appDbContext;

        public Handler(IAppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<HandlerResult<List<CategoryDto>>> Handle(Unit _)
        {
            var result = await _appDbContext.Categories.Select(x => new CategoryDto
            {
                Id = x.Id,
                Name = x.Name,
                ParentCategoryId = x.ParentCategoryId,
                CreatedAt = x.CreatedAt,
                ModifiedAt = x.ModifiedAt
            }).ToListAsync();

            return HandlerResult<List<CategoryDto>>.Ok(result);
        }
    }
}
