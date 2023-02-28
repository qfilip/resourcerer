using MediatR;
using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using System.Text.Json;

namespace Resourcerer.Logic.Elements.Queries;
public static class ElementOverviews
{
    public class Query : IRequest<string> { }

    public class Handler : IRequestHandler<Query, string>
    {
        private readonly AppDbContext _appDbContext;

        public Handler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<string> Handle(Query request, CancellationToken cancellationToken)
        {
            var query =
                from es in _appDbContext.Elements
                join ps in _appDbContext.ElementPurchasedEvents
                on es.Id equals ps.ElementId
                into elementsWithEvents
                select new { Element = elementsWithEvents };

            var data = await query.ToListAsync();
            
            return JsonSerializer.Serialize(data);
        }
    }
}
