using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos;
using Resourcerer.Logic.Functions.V1_0;

namespace Resourcerer.Logic.Commands.V1_0;

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
