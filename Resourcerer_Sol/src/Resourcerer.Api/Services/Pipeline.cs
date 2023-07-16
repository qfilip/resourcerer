using Resourcerer.Dtos;
using Resourcerer.Logic;

namespace Resourcerer.Api.Services;

public class Pipeline
{
    private readonly ILogger<Pipeline> _logger;

    public Pipeline(ILogger<Pipeline> logger)
    {
        _logger = logger;
    }

    public async Task<IResult> PipeGet<TRequest, TResponse>(
        IAppHandler<TRequest, TResponse> handler,
        TRequest request,
        Func<TResponse, IResult>? customOkResultMapper = null)
    {
        var actionName = GetHandlerName(handler);

        _logger.LogInformation("Action {Action} started", actionName);

        var handlerResult = await handler.Handle(request);

        _logger.LogInformation("Action {Action} finished", actionName);

        return MapResult(handlerResult, customOkResultMapper);
    }

    public async Task<IResult> Pipe<TRequest, TResponse>(
        IAppHandler<TRequest, TResponse> handler,
        TRequest request,
        Func<TResponse, IResult>? customOkResultMapper = null)
        where TRequest : BaseDto<TRequest>
    {
        var actionName = GetHandlerName(handler);

        _logger.LogInformation("Action {Action} started", actionName);

        var validationErrors = DtoValidator.Validate(request, request.GetValidator());
        if (validationErrors.Any())
        {
            _logger.LogInformation("Action {Action} finished with validation errors", actionName);
            return Results.BadRequest(validationErrors);
        }

        var handlerResult = await handler.Handle(request);

        _logger.LogInformation("Action {Action} finished", actionName);

        return MapResult(handlerResult, customOkResultMapper);
    }

    private static IResult MapResult<TResponse>(
        HandlerResult<TResponse> handlerResult,
        Func<TResponse, IResult>? customOkResultMapper = null)
    {
        return (handlerResult.Status, customOkResultMapper) switch
        {
            (eHandlerResultStatus.Ok, null) => Results.Ok(handlerResult.Object),
            (eHandlerResultStatus.Ok, _) => customOkResultMapper.Invoke(handlerResult.Object!),
            (eHandlerResultStatus.NotFound, _) => Results.NotFound(handlerResult.Object),
            (eHandlerResultStatus.ValidationError, _) => Results.BadRequest(handlerResult.Errors),
            _ =>
                throw new NotImplementedException($"Cannot handle status of {handlerResult.Status}")
        };
    }

    private string GetHandlerName(object handler) =>
        handler.GetType().FullName!.Split('.').Last();
}
