﻿using Microsoft.AspNetCore.Mvc;
using Resourcerer.Dtos.UnitsOfMeasure;
using Resourcerer.Logic.UnitsOfMeasure.Command;

namespace Resourcerer.Api.Endpoints.UnitsOfMeasure;

public class AddUnitOfMeasureEndpoint
{
    public static async Task<IResult> Action(
       [FromBody] UnitOfMeasureDto dto,
       [FromServices] Pipeline pipeline,
       [FromServices] AddUnitOfMeasure.Handler handler)
    {
        return await pipeline.Pipe(handler, dto, nameof(AddUnitOfMeasure));
    }
}