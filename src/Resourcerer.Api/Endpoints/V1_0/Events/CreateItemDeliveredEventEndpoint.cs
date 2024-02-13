using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Dtos;
using Resourcerer.Dtos.Events;
using Resourcerer.Logic.Commands.V1_0;
using System.Threading.Channels;

namespace Resourcerer.Api.Endpoints.V1_0;

public class CreateItemDeliveredEventEndpoint
{
    public static async Task<IResult> Action(
        [FromBody] InstanceDeliveredRequestDto dto,
        [FromServices] ChannelWriter<EventDtoBase> writer,
        [FromServices] Pipeline pipeline)
    {
        return await pipeline.PipeToChannel(
            dto,
            CreateInstanceDeliveredEvent.Handler.ValidateRequest,
            writer,
            nameof(CreateInstanceDeliveredEvent));
    }

    internal static void MapToGroup(RouteGroupBuilder group)
    {
        var endpoint = group.MapPost("/order-deliver", Action);

        EndpointMapper.AddAuthorization(endpoint, new List<(ePermissionSection claimType, ePermission[] claimValues)>
        {
            (ePermissionSection.Event, new[] { ePermission.Write })
        });
    }
}
