using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Dtos;
using Resourcerer.Dtos.Instances.Events.Order;
using Resourcerer.Logic.V1_0.Commands;

namespace Resourcerer.Api.Endpoints.V1_0;

public class CreateInstanceOrderSentEventEndpoint
{
    public static async Task<IResult> Action(
        [FromBody] InstanceOrderSentRequestDto dto,
        [FromServices] ISenderAdapter<InstanceOrderEventDtoBase> sender,
        [FromServices] Pipeline pipeline)
    {
        return await pipeline.PipeMessage(
            dto,
            () => CreateInstanceOrderSentEvent.Handler.Validate(dto),
            sender,
            nameof(CreateInstanceOrderSentEvent));
    }

    internal static void MapToGroup(RouteGroupBuilder group)
    {
        var endpoint = group.MapPost("/order-sent", Action);

        EndpointMapper.AddAuthorization(endpoint, new List<(ePermissionSection claimType, ePermission[] claimValues)>
        {
            (ePermissionSection.Event, new[] { ePermission.Write })
        });
    }
}
