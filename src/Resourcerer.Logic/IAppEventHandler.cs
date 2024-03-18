namespace Resourcerer.Logic;

public interface IAppEventHandler<TRequest, TResponse>
{
    Task<HandlerResult<TResponse>> Handle(TRequest request);
}
