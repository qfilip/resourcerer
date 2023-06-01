using MediatR;
using Resourcerer.Logic.Elements.Queries;
using Resourcerer.Logic.Mocks.Commands;
using Resourcerer.Logic.Mocks.Queries;
using System.Text.Json;

namespace Resourcerer.Api.Endpoints;

public class Mocks
{
    private static async Task<IResult> Seed(IMediator mediatr)
    {
        var dbdata = await mediatr.Send(new GetMockDatabaseData.Query());
        await mediatr.Send(new SeedMockData.Command(dbdata));
        return Results.Ok();
    }

    private static async Task<IResult> GetMockData(IMediator mediatr)
    {
        var dbdata = await mediatr.Send(new GetMockDatabaseData.Query());
        var json = JsonSerializer.Serialize(dbdata);
        return Results.Ok(json);
    }

    private static async Task<IResult> TestQuery(IMediator mediatr)
    {
        var data = await mediatr.Send(new ElementOverviews.Query());
        return Results.Ok(data);
    }

    public static void MapEndpoints(WebApplication app)
    {
        app.MapGet("mock/seeddb", Seed);
        app.MapGet("mock/data", GetMockData);
        app.MapGet("mock/testquery", TestQuery);
    }
}

