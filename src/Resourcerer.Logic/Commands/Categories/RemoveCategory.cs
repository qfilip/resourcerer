using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Enums;
using Resourcerer.Dtos;
using Resourcerer.Logic.Functions.Common;

namespace Resourcerer.Logic.Commands.Categories;

public static class RemoveCategory
{
    public class Handler : IAppHandler<CategoryDto, Guid>
    {
        private readonly AppDbContext _dbContext;
        public Handler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<HandlerResult<Guid>> Handle(CategoryDto request)
        {
            return await EntityAction
                .Remove<Category>(_dbContext, request.Id, $"Category with id {request.Id} doesn't exist");
        }
    }
}
