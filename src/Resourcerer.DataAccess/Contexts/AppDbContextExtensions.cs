using Resourcerer.DataAccess.Abstractions;
using Resourcerer.DataAccess.Enums;

namespace Resourcerer.DataAccess.Contexts;

public static class AppDbContextExtensions
{
    public static void MarkAsDeleted<T>(this AppDbContext appDbContext, T entity)
        where T : class, ISoftDeletable
    {
        entity.EntityStatus = eEntityStatus.Deleted;
        appDbContext.Entry(entity).Property(x => x.EntityStatus).IsModified = true;
    }
}
