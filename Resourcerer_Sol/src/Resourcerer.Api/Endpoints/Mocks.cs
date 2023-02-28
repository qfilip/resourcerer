using MediatR;
using Resourcerer.Logic.Elements.Queries;
using Resourcerer.Logic.Mocks.Commands;

namespace Resourcerer.Api.Endpoints;

public class Mocks
{
    private static async Task<IResult> Seed(IMediator mediatr)
    {
        await mediatr.Send(new SeedMockData.Command());
        return Results.Ok();
    }

    private static async Task<IResult> TestQuery(IMediator mediatr)
    {
        var data = await mediatr.Send(new ElementOverviews.Query());
        return Results.Ok(data);
    }

    public static void MapEndpoints(WebApplication app)
    {
        app.MapPost("mock/seeddb", Seed);
        app.MapPost("mock/testquery", TestQuery);
    }
}

