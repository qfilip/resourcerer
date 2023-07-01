using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos;
using Resourcerer.Dtos.Elements;
using Resourcerer.Logic;
using Resourcerer.Logic.Commands.Elements;

namespace Resourcerer.Api.Endpoints.V1_0.Elements;

public class AddElementEndpoint
{
    public static async Task<IResult> Action(
       [FromBody] ElementDto dto,
       [FromServices] Pipeline pipeline,
       [FromServices] CreateElement.Handler handler)
    {
        return await pipeline
            .Pipe<ElementDto, ElementDtoValidator, Unit>(handler, dto);
    }

    internal static void MapToGroup(RouteGroupBuilder group)
    {
        var endpoint = group.MapPost("/add", Action);

        EndpointMapper.AddAuthorization(endpoint, new List<(string claimType, string[] claimValues)>
        {
            (nameof(Element), new[] { ePermission.Write.ToString() })
        });
    }
}
