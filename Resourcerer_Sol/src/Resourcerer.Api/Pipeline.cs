using Resourcerer.Logic;

namespace Resourcerer.Api;

public class Pipeline
{
    private readonly ILogger<Pipeline> _logger;

    public Pipeline(ILogger<Pipeline> logger)
    {
        _logger = logger;
    }

    public async Task<IResult> Pipe<TRequest, TResponse>(
        IRequestHandler<TRequest, TResponse> handler,
        TRequest request,
        string actionName,
        Func<HandlerResult<TResponse>, IResult>? customResultMapper = null)
    {
        _logger.LogInformation("Action {Action} started", actionName);
        var handlerResult = await handler.Handle(request);
        _logger.LogInformation("Action {Action} finished", actionName);

        var result = customResultMapper?.Invoke(handlerResult);

        return (handlerResult.Status, result) switch
        {
            (_, IResult) => result,
            (HandlerResultStatus.Ok, null) => Results.Ok(handlerResult.Object),
            (HandlerResultStatus.NotFound, null) => Results.NotFound(handlerResult.Object),
            (HandlerResultStatus.ValidationError, null) => Results.BadRequest(handlerResult.Errors),
            _ =>
                throw new NotImplementedException($"Cannot handle status of {handlerResult.Status}")
        };
    }
}
