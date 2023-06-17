namespace Resourcerer.Logic;

public interface IRequestHandler<TRequest, TResponse>
{
    public Task<HandlerResult<TResponse>> Handle(TRequest request);
}
