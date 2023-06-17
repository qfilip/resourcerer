using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using System.Text.Json;

namespace Resourcerer.Logic.Elements.Queries;
public static class GetAllElementsOverview
{
    public class Handler : IRequestHandler<Unit, string>
    {
        private readonly AppDbContext _appDbContext;

        public Handler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<HandlerResult<string>> Handle(Unit _)
        {
            var query =
                from es in _appDbContext.Elements
                join ps in _appDbContext.ElementPurchasedEvents
                on es.Id equals ps.ElementId
                into elementsWithEvents
                select new { Element = elementsWithEvents };

            var data = await query.ToListAsync();
            var stringData = JsonSerializer.Serialize(data);
            
            return HandlerResult<string>.Ok(stringData);
        }
    }
}
