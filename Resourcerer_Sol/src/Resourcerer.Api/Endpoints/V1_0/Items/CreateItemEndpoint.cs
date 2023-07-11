using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Dtos;
using Resourcerer.Logic.Commands.Items;

namespace Resourcerer.Api.Endpoints.V1_0.Items;

public class CreateItemEndpoint
{
    public static async Task<IResult> Action(
       [FromBody] CreateItemDto dto,
       [FromServices] Pipeline pipeline,
       [FromServices] CreateItem.Handler handler)
    {
        return await pipeline.Pipe(handler, dto);
    }

    internal static void MapToGroup(RouteGroupBuilder group)
    {
        var endpoint = group.MapPost("/create", Action);

        EndpointMapper.AddAuthorization(endpoint, new List<(eSection claimType, ePermission[] claimValues)>
        {
            (eSection.Item, new[] { ePermission.Write })
        });
    }
}
