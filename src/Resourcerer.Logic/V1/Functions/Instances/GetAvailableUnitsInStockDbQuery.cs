using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Entities;

namespace Resourcerer.Logic.V1.Functions;

public static partial class Instances
{
    public static IQueryable<Instance> GetAvailableUnitsInStockDbQuery(DbSet<Instance> dbSet) =>
        dbSet
            .Include(x => x.SourceInstance)
                .ThenInclude(x => x!.OrderedEvents)
            .Include(x => x.OrderedEvents)
            .Include(x => x.ReservedEvents)
            .Include(x => x.DiscardedEvents);
    
}
