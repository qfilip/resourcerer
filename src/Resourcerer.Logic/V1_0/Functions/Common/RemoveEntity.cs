using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Enums;

namespace Resourcerer.Logic.Functions.V1_0;

internal partial class EntityAction
{
    public static async Task<HandlerResult<Guid>> Remove<T>(AppDbContext context, Guid id, string notFoundMessage) where T : EntityBase
    {
        var dbSet = context.Set<T>();
        var entity = (await dbSet.FirstOrDefaultAsync(x => x.Id == id)) as EntityBase;

        if (entity == null)
        {
            return HandlerResult<Guid>.NotFound(notFoundMessage);
        }

        entity.EntityStatus = eEntityStatus.Deleted;
        await context.SaveChangesAsync();

        return HandlerResult<Guid>.Ok(entity.Id);
    }
}
