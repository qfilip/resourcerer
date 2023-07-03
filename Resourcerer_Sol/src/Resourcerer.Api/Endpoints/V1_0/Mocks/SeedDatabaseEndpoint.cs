﻿using Resourcerer.Api.Services;
using Resourcerer.DataAccess.Mocks;
using Resourcerer.Logic.Commands.Mocks;

namespace Resourcerer.Api.Endpoints.V1_0.Mocks;

public class SeedDatabaseEndpoint
{
    public static async Task<IResult> Action(
        DatabaseData dto,
        Pipeline pipeline,
        SeedMockData.Handler seedHandler)
    {
        return await pipeline.Pipe(seedHandler, dto);
    }

    internal static void MapToGroup(RouteGroupBuilder group)
    {
        group.MapPost("/seed", Action);
    }
}
