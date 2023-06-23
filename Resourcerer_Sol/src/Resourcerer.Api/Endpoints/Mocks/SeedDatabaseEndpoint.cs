using Resourcerer.Logic;
using Resourcerer.Logic.Mocks.Commands;
using Resourcerer.Logic.Mocks.Queries;

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
            return await pipeline.Pipe(seedHandler, dbData.Object, nameof(SeedMockData));
        }
        else
        {
            return Results.BadRequest();
        }
    }
}
