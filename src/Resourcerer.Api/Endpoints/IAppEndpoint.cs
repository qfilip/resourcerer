namespace Resourcerer.Api.Endpoints;

internal interface IAppEndpoint
{
    AppEndpoint GetEndpointInfo();
}

// used for testing
internal interface IAppTestEndpoint
{
    AppEndpoint GetEndpointInfo();
}
