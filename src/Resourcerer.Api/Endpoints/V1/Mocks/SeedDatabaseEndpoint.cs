using Resourcerer.Api.Services;
using Resourcerer.Logic;
using Resourcerer.Logic.V1.Commands;

namespace Resourcerer.Api.Endpoints.V1;

public class SeedDatabaseEndpoint
{
    public static Task<IResult> Action()
    {
        return Task.FromResult(Results.Ok());
    }

    internal static void MapToGroup(RouteGroupBuilder group)
    {
        group.MapPost("/seed", Action);
    }
}
