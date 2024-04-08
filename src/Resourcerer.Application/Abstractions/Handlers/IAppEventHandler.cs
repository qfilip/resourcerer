using Resourcerer.Application.Models;

namespace Resourcerer.Application.Abstractions.Handlers;

public interface IAppEventHandler<TRequest, TResponse>
{
    Task<HandlerResult<TResponse>> Handle(TRequest request);
}
