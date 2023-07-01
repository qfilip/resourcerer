using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Mocks;

namespace Resourcerer.Logic.Queries.Mocks;
public class GetMockedDatabaseData
{
    public class Handler : MockingUtilities, IRequestHandler<Unit, DatabaseData>
    {
        public Task<HandlerResult<DatabaseData>> Handle(Unit _)
        {
            var dbData = base.GetData();

            return Task.FromResult(HandlerResult<DatabaseData>.Ok(dbData));
        }
    }
}

