using Resourcerer.DataAccess.Contexts;
using Resourcerer.Dtos;

namespace Resourcerer.Logic.Commands.Items;

public static class CreateCompositeItem
{
    public class Handler : IAppHandler<CreateCompositeItemDto, Unit>
    {
        private readonly AppDbContext _appDbContext;

        public Handler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public Task<HandlerResult<Unit>> Handle(CreateCompositeItemDto request)
        {
            throw new NotImplementedException();
        }
    }
}
