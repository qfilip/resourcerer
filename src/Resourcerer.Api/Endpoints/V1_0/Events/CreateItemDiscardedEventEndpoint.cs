using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Dtos;
using Resourcerer.Dtos.Instances.Events;
using Resourcerer.Dtos.Instances.Events.Order;
using Resourcerer.Logic.Commands.V1_0;
using Resourcerer.Logic.V1_0.Commands;
using System.Threading.Channels;

namespace Resourcerer.Api.Endpoints.V1_0;

public class CreateItemDiscardedEventEndpoint
{
    public static async Task<IResult> Action(
        [FromBody] InstanceDiscardedRequestDto dto,
        [FromServices] ChannelWriter<InstanceOrderEventDtoBase> writer,
        [FromServices] Pipeline pipeline)
    {
        return await pipeline.PipeToChannel(
            dto,
            CreateInstanceDiscardedEvent.Handler.ValidateRequest,
            writer,
            nameof(CreateInstanceDiscardedEvent));
    }

    internal static void MapToGroup(RouteGroupBuilder group)
    {
        var endpoint = group.MapPost("/discard", Action);

        EndpointMapper.AddAuthorization(endpoint, new List<(ePermissionSection claimType, ePermission[] claimValues)>
        {
            (ePermissionSection.Event, new[] { ePermission.Write })
        });
    }
}
