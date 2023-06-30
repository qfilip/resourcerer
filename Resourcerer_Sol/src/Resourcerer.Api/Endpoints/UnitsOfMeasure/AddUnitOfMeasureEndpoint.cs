using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos;
using Resourcerer.Dtos.UnitsOfMeasure;
using Resourcerer.Logic;
using Resourcerer.Logic.Commands.UnitsOfMeasure;

namespace Resourcerer.Api.Endpoints.UnitsOfMeasure;

public class AddUnitOfMeasureEndpoint
{
    public static async Task<IResult> Action(
       [FromBody] UnitOfMeasureDto dto,
       [FromServices] Pipeline pipeline,
       [FromServices] AddUnitOfMeasure.Handler handler)
    {
        return await pipeline
            .Pipe<UnitOfMeasureDto, UnitOfMeasureDtoValidator, Unit>(handler, dto);
    }

    internal static void MapToGroup(RouteGroupBuilder group)
    {
        group.MapPost("/add", Action)
            .RequireAuthorization(cfg =>
            {
                cfg.RequireClaim(nameof(UnitOfMeasure), ePermission.Write.ToString());
            });
    }
}
