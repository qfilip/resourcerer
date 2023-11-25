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
        return await pipeline.Pipe(handler, dto, new CreateElementItemDto.Validator());
    }

    internal static void MapToGroup(RouteGroupBuilder group)
    {
        var endpoint = group.MapPost("/create-element", Action);

        EndpointMapper.AddAuthorization(endpoint, new List<(ePermissionSection claimType, ePermission[] claimValues)>
        {
            (ePermissionSection.Event, new[] { ePermission.Write })
        });
    }
}
