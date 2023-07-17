using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Dtos;
using Resourcerer.Logic.Commands.Items;

namespace Resourcerer.Api.Endpoints.V1_0.Items;

public class CreateElementItemEndpoint
{
    public static async Task<IResult> Action(
       [FromBody] CreateElementItemDto dto,
       [FromServices] Pipeline pipeline,
       [FromServices] CreateElementItem.Handler handler)
    {
        return await pipeline.PipeWithValidator(handler, dto);
    }

    internal static void MapToGroup(RouteGroupBuilder group)
    {
        var endpoint = group.MapPost("/create-element", Action);

        EndpointMapper.AddAuthorization(endpoint, new List<(eSection claimType, ePermission[] claimValues)>
        {
            (eSection.Item, new[] { ePermission.Write })
        });
    }
}
