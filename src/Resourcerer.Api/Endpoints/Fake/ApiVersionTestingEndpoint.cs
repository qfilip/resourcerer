namespace Resourcerer.Api.Endpoints.Fake;
using HttpMethod = Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http.HttpMethod;

public class ApiVersionTestingEndpoint : IAppTestEndpoint
{
    public static IResult Action() => Results.Ok("v1.0");
    public AppEndpoint GetEndpointInfo()
    {
        return new AppEndpoint(1, 0, EndpointMapper.Fake("api"), HttpMethod.Get, Action, null);
    }
}

public class ApiVersionTestingEndpoint1 : IAppTestEndpoint
{
    public static IResult Action() => Results.Ok("v1.1");
    public AppEndpoint GetEndpointInfo()
    {
        return new AppEndpoint(1, 1, EndpointMapper.Fake("api"), HttpMethod.Get, Action, null);
    }
}

public class ApiVersionTestingEndpoint2 : IAppTestEndpoint
{
    public static IResult Action() => Results.Ok("v1.3");
    public AppEndpoint GetEndpointInfo()
    {
        return new AppEndpoint(1, 3, EndpointMapper.Fake("api"), HttpMethod.Get, Action, null);
    }
}

public class ApiVersionTestingEndpoint3 : IAppTestEndpoint
{
    public static IResult Action() => Results.Ok("v2.0");
    public AppEndpoint GetEndpointInfo()
    {
        return new AppEndpoint(2, 0, EndpointMapper.Fake("api"), HttpMethod.Get, Action, null);
    }
}

public class ApiVersionTestingEndpoint4 : IAppTestEndpoint
{
    public static IResult Action() => Results.Ok("v2.5");
    public AppEndpoint GetEndpointInfo()
    {
        return new AppEndpoint(2, 5, EndpointMapper.Fake("api"), HttpMethod.Get, Action, null);
    }
}