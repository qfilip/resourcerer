﻿using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos;
using Resourcerer.Dtos.UnitsOfMeasure;
using Resourcerer.Logic;
using Resourcerer.Logic.Commands.UnitsOfMeasure;

namespace Resourcerer.Api.Endpoints.V1_0.UnitsOfMeasure;

public class CreateUnitOfMeasureEndpoint
{
    public static async Task<IResult> Action(
       [FromBody] UnitOfMeasureDto dto,
       [FromServices] Pipeline pipeline,
       [FromServices] CreateUnitOfMeasure.Handler handler)
    {
        return await pipeline
            .Pipe<UnitOfMeasureDto, UnitOfMeasureDtoValidator, Unit>(handler, dto);
    }

    internal static void MapToGroup(RouteGroupBuilder group)
    {
        var endpoint = group.MapPost("/create", Action);

        EndpointMapper.AddAuthorization(endpoint, new List<(string claimType, string[] claimValues)>
        {
            (nameof(UnitOfMeasure), new[] { ePermission.Write.ToString() })
        });
    }
}