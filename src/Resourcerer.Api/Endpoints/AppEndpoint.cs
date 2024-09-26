namespace Resourcerer.Api.Endpoints;

public record AppEndpoint(
    int Major,
    int Minor,
    string Path,
    eHttpMethod Method,
    Delegate EndpointAction,
    Action<RouteHandlerBuilder>? MapAuth = null);
