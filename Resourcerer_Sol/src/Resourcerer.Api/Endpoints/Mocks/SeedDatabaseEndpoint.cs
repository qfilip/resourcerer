using Resourcerer.Logic;
using Resourcerer.Logic.Commands.Mocks;
using Resourcerer.Logic.Queries.Mocks;

namespace Resourcerer.Api.Endpoints.Mocks;

public class SeedDatabaseEndpoint
{
    public static async Task<IResult> Action(
        Pipeline pipeline,
        GetMockedDatabaseData.Handler mockHandler,
        SeedMockData.Handler seedHandler)
    {
        var dbData = await mockHandler.Handle(new Unit());
        if(dbData.Object != null)
        {
            return await pipeline.Pipe(seedHandler, dbData.Object);
        }
        else
        {
            return Results.BadRequest();
        }
    }
}
