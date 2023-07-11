using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.Elements;
using Resourcerer.Utilities;

namespace Resourcerer.Logic.Queries.Items;
public static class GetItemsStatistics
{
    public class Handler : IAppHandler<Unit, List<ItemStatisticsDto>>
    {
        private readonly AppDbContext _appDbContext;

        public Handler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<HandlerResult<List<ItemStatisticsDto>>> Handle(Unit _)
        {
            //var item = await _appDbContext.Items
            //    .Include(x => x.Category)
            //    .Include(x => x.UnitOfMeasure)
            //    .Include(x => x.Prices)
            //    .Include(x => x.ElementExcerpts)
            //    .Include(x => x.CompositeExcerpts)
            //    .Include(x => x.Instances)
            //        .ThenInclude(x => x.InstanceOrderedEvent)
            //    .Include(x => x.Instances)
            //        .ThenInclude(x => x.InstanceDeliveredEvent)
            //    .Include(x => x.Instances)
            //        .ThenInclude(x => x.InstanceDiscardedEvent)
            //    .Include(x => x.Instances)
            //        .ThenInclude(x => x.InstanceOrderCancelledEvent)
            //    .FirstOrDefaultAsync();

            //if(item == null)
            //{
            //    return HandlerResult<List<ItemStatisticsDto>>.Ok(new List<ItemStatisticsDto>());
            //}

            //var pendingInstances = item.Instances
            //    .Where(x => x.InstanceOrderedEvents)
            
            return HandlerResult<List<ItemStatisticsDto>>.Ok(new List<ItemStatisticsDto>());
        }
    }
}
