﻿using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos;
using Resourcerer.Dtos.Composites;
using Resourcerer.Logic;
using Resourcerer.Logic.Commands.Composites;

namespace Resourcerer.Api.Endpoints.V1_0.Composites;

public class CreateCompositeEndpoint
{
    public static async Task<IResult> Action(
        [FromBody] CompositeDto dto,
        [FromServices] Pipeline pipeline,
        [FromServices] CreateComposite.Handler handler)
    {
        return await pipeline.Pipe<CompositeDto, CompositeDtoValidator, Unit>(handler, dto);
    }

    internal static void MapToGroup(RouteGroupBuilder group)
    {
        var endpoint = group.MapPost("/create", Action);

        EndpointMapper.AddAuthorization(endpoint, new List<(string claimType, string[] claimValues)>
        {
            (nameof(Composite), new[] { ePermission.Write.ToString() })
        });
    }
}
