using Microsoft.AspNetCore.Mvc;
using Resourcerer.Dtos;
using Resourcerer.Dtos.Events;
using System.Threading.Channels;

namespace Resourcerer.Api.Endpoints.V1_0;

public class CreateItemDeliveredEventEndpoint
{
    public static async Task<IResult> Action(
        [FromBody] ItemDeliveredEventDto dto,
         ChannelWriter<ItemEventDtoBase> writer)
    {
        await writer.WriteAsync(dto);
        return Results.Accepted();
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
