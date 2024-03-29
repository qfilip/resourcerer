﻿using Resourcerer.Api.Services;

namespace Resourcerer.Api.Endpoints.V1;

public static class RefreshSessionEndpoint
{
    public static IResult Action(HttpContext context)
    {
        var claims = context.User.Claims;
        var token = JwtService.RefreshToken(claims);

        return Results.Ok(token);
    }

    internal static void MapToGroup(RouteGroupBuilder group)
    {
        var endpoint = group.MapGet("/refresh_session", Action);
        EndpointMapper.AddAuthorization(endpoint);
    }
}