using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.Composites;
using Resourcerer.Dtos;
using Resourcerer.Logic.Commands.Composites;
using Resourcerer.Logic;
using Resourcerer.Dtos.OldPrices;

namespace Resourcerer.Api.Endpoints.V1_0.Composites;

public class ChangeCompositePriceEndpoint
{
    public static async Task<IResult> Action(
        [FromBody] ChangePriceDto dto,
        [FromServices] Pipeline pipeline,
        [FromServices] ChangeCompositePrice.Handler handler)
    {
        return await pipeline.Pipe<ChangePriceDto, ChangePriceDtoValidator, Unit>(handler, dto);
    }

    internal static void MapToGroup(RouteGroupBuilder group)
    {
        var endpoint = group.MapPost("/change-price", Action);

        EndpointMapper.AddAuthorization(endpoint, new List<(string claimType, string[] claimValues)>
        {
            (nameof(Composite), new[] { ePermission.Write.ToString() })
        });
    }
}
