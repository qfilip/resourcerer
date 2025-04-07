using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Dtos.V1;
using Resourcerer.Identity.Enums;
using Resourcerer.Logic.V1;
using Resourcerer.Messaging.Abstractions;

namespace Resourcerer.Api.Endpoints.V1;

public class UpdateExampleEndpoint : IAppEndpoint
{
    public static Task<IResult> Action(
    [FromBody] V1UpdateExampleCommand command,
    [FromServices] UpdateExample.Validator validator,
    [FromServices] IMessageSender<V1ExampleCommand> sender,
    [FromServices] Pipeline pipeline
    ) => pipeline.PipeMessage(command, validator, sender, nameof(UpdateExample));

    internal static void MapAuth(RouteHandlerBuilder endpoint)
    {
        EndpointMapper.AddAuthorization(endpoint, new List<(eResource claimType, eAction[] claimValues)>
        {
            (eResource.Example, new[] { eAction.Update })
        });
    }

    public AppEndpoint GetEndpointInfo() =>
        new AppEndpoint(
            1, 0,
            EndpointMapper.Examples("/update"),
            eHttpMethod.Post,
            Action,
            MapAuth
        );
}
