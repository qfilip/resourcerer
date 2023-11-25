namespace Resourcerer.Logic;

public interface IAppHandler<TRequest, TResponse>
{
    Task<HandlerResult<TResponse>> Handle(TRequest request);
}
