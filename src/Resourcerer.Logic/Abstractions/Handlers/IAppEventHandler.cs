using Resourcerer.Logic.Models;

namespace Resourcerer.Application.Logic.Handlers;

public interface IAppEventHandler<TRequest, TResponse>
{
    Task<HandlerResult<TResponse>> Handle(TRequest request);
}
