using Resourcerer.DataAccess.Contexts;
using Resourcerer.Dtos.Items;

namespace Resourcerer.Logic.Queries.Items;

public static class GetItemStockInfo
{
    public class Handler : IAppHandler<(Guid itemId, DateTime now), ItemStockInfoDto>
    {
        private readonly AppDbContext _appDbContext;

        public Handler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public Task<HandlerResult<ItemStockInfoDto>> Handle((Guid itemId, DateTime now) request)
        {
            throw new NotImplementedException();
        }
    }
}
