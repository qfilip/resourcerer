using Microsoft.AspNetCore.Mvc;
using Resourcerer.Api.Services;
using Resourcerer.Logic.V1;

namespace Resourcerer.Api.Endpoints.V1;

public class GetItemShoppingListEndpoint : IAppEndpoint
{
    public static async Task<IResult> Action(
       [FromQuery] Guid companyId,
       [FromServices] Pipeline pipeline,
       [FromServices] GetItemShoppingList.Handler handler)
    {
        return await pipeline.Pipe(handler, companyId);
    }

    public AppEndpoint GetEndpointInfo() =>
        new AppEndpoint(1, 0,
            EndpointMapper.Items("shopping-list"), eHttpMethod.Get, Action);
}
