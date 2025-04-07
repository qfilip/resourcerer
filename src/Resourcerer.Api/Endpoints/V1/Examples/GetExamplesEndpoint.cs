using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Identity.Enums;
using Resourcerer.Logic.Models;
using Resourcerer.Logic.V1;

namespace Resourcerer.Api.Endpoints.V1;

public class GetExamplesEndpoint : IAppEndpoint
{
    public static Task<IResult> Action(
    [FromServices] GetExamples.Handler handler,
    [FromServices] Pipeline pipeline
    ) => pipeline.Pipe(handler, Unit.New);

    internal static void MapAuth(RouteHandlerBuilder endpoint)
    {
        EndpointMapper.AddAuthorization(endpoint, new List<(eResource claimType, eAction[] claimValues)>
        {
            (eResource.Example, new[] { eAction.View })
        });
    }

    public AppEndpoint GetEndpointInfo() =>
        new AppEndpoint(
            1, 0,
            EndpointMapper.Examples(""),
            eHttpMethod.Get,
            Action,
            MapAuth
        );
}
