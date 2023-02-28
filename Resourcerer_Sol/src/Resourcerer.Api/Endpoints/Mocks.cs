using MediatR;
using Resourcerer.Logic.Mocks.Commands;

namespace Resourcerer.Api.Endpoints;

public class Mocks
{
    private static async Task<IResult> Seed(IMediator mediatr)
    {
        await mediatr.Send(new SeedMockData.Command());
        return Results.Ok();
    }

    public static void MapEndpoints(WebApplication app)
    {
        app.MapPost("mock/seeddb", Seed);
    }
}

