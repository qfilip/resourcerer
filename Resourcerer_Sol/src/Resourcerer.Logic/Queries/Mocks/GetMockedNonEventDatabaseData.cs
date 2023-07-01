using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Mocks;

namespace Resourcerer.Logic.Queries.Mocks;

public static class GetMockedNonEventDatabaseData
{
    public class Handler : MockingUtilities, IRequestHandler<Unit, DatabaseData>
    {
        public Task<HandlerResult<DatabaseData>> Handle(Unit _)
        {
            var dbData = base.GetData();
            dbData.CompositeSoldEvents = Array.Empty<CompositeSoldEvent>();
            dbData.ElementPurchasedEvents = Array.Empty<ElementPurchasedEvent>();


            var dbdata = new DatabaseData
            {
                AppUsers = users,
                Categories = categories,
                Excerpts = excerpts,
                UnitOfMeasures = uoms,
                OldPrices = prices,

                Composites = composites,
                CompositeSoldEvents = Array.Empty<CompositeSoldEvent>(),

                Elements = elements,
                ElementSoldEvents = Array.Empty<ElementSoldEvent>(),
                ElementPurchasedEvents = Array.Empty<ElementPurchasedEvent>()
            };

            return Task.FromResult(HandlerResult<DatabaseData>.Ok(dbdata));
        }
    }
}
