namespace Resourcerer.Logic;

public interface IRequestHandler<TRequest, TResponse>
{
    Task<HandlerResult<TResponse>> Handle(TRequest request);
}
