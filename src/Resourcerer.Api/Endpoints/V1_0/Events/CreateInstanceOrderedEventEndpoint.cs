using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Dtos;
using Resourcerer.Dtos.Instances.Events.Order;
using Resourcerer.Logic.Commands.V1_0;
using System.Threading.Channels;

namespace Resourcerer.Api.Endpoints.V1_0;

public class CreateInstanceOrderedEventEndpoint
{
    public static async Task<IResult> Action(
        [FromBody] InstanceOrderRequestDto dto,
        [FromServices] ChannelWriter<InstanceOrderEventDtoBase> writer,
        [FromServices] Pipeline pipeline)
    {
        return await pipeline.PipeToChannel(
            dto,
            CreateInstanceOrderedEvent.Handler.ValidateRequest,
            writer,
            nameof(CreateInstanceOrderedEvent));
    }

    internal static void MapToGroup(RouteGroupBuilder group)
    {
        var endpoint = group.MapPost("/order", Action);

        EndpointMapper.AddAuthorization(endpoint, new List<(ePermissionSection claimType, ePermission[] claimValues)>
        {
            (ePermissionSection.Event, new[] { ePermission.Write })
        });
    }
}
