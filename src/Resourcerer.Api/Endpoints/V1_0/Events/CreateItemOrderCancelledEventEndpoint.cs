using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Dtos;
using Resourcerer.Dtos.Instances.Events.Order;
using Resourcerer.Logic.Commands.V1_0;
using System.Threading.Channels;

namespace Resourcerer.Api.Endpoints.V1_0;

public class CreateItemOrderCancelledEventEndpoint
{
    public static async Task<IResult> Action(
        [FromBody] InstanceOrderCancelRequestDto dto,
        [FromServices] ChannelWriter<InstanceOrderEventDtoBase> writer,
        [FromServices] Pipeline pipeline)
    {
        return await pipeline.PipeToChannel(
            dto,
            CreateInstanceOrderCancelledEvent.Handler.ValidateRequest,
            writer,
            nameof(CreateInstanceOrderCancelledEvent));
    }

    internal static void MapToGroup(RouteGroupBuilder group)
    {
        var endpoint = group.MapPost("/order-cancel", Action);

        EndpointMapper.AddAuthorization(endpoint, new List<(ePermissionSection claimType, ePermission[] claimValues)>
        {
            (ePermissionSection.Event, new[] { ePermission.Write })
        });
    }
}
