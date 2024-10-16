using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Enums;
using System.Runtime.CompilerServices;

namespace Resourcerer.DataAccess.Contexts;

public static class AppDbContextExtensions
{
    public static void MarkAsDeleted<T>(this AppDbContext appDbContext, T entity)
        where T : AppDbEntity
    {
        entity.EntityStatus = eEntityStatus.Deleted;
        appDbContext.Entry(entity).Property(x => x.EntityStatus).IsModified = true;
    }
}
