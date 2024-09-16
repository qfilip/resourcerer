using HttpMethod = Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http.HttpMethod;

namespace Resourcerer.Api.Endpoints;

public record AppEndpoint(
    int Major,
    int Minor,
    string Path,
    HttpMethod Method,
    Delegate EndpointAction,
    Action<RouteHandlerBuilder>? MapAuth = null);
