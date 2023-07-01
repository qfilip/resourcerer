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
            dbData.ElementSoldEvents = Array.Empty<ElementSoldEvent>();

            return Task.FromResult(HandlerResult<DatabaseData>.Ok(dbData));
        }
    }
}
