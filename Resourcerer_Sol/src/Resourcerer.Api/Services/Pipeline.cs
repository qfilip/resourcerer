using FluentValidation;
using Resourcerer.Logic;

namespace Resourcerer.Api.Services;

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
        Func<TResponse, IResult>? customOkResultMapper = null)
    {
        var actionName = GetHandlerName(handler);

        _logger.LogInformation("Action {Action} started", actionName);

        var handlerResult = await handler.Handle(request);

        _logger.LogInformation("Action {Action} finished", actionName);

        return MapResult(handlerResult, customOkResultMapper);
    }

    public async Task<IResult> Pipe<TRequest, TRequestValidator, TResponse>(
        IRequestHandler<TRequest, TResponse> handler,
        TRequest request,
        Func<TResponse, IResult>? customOkResultMapper = null)
        where TRequest : class
        where TRequestValidator : AbstractValidator<TRequest>, new()
    {
        var actionName = GetHandlerName(handler);

        _logger.LogInformation("Action {Action} started", actionName);

        var validationErrors = DtoValidator.Validate<TRequest, TRequestValidator>(request);
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
            (HandlerResultStatus.Ok, null) => Results.Ok(handlerResult.Object),
            (HandlerResultStatus.Ok, _) => customOkResultMapper.Invoke(handlerResult.Object!),
            (HandlerResultStatus.NotFound, _) => Results.NotFound(handlerResult.Object),
            (HandlerResultStatus.ValidationError, _) => Results.BadRequest(handlerResult.Errors),
            _ =>
                throw new NotImplementedException($"Cannot handle status of {handlerResult.Status}")
        };
    }

    private string GetHandlerName(object handler) =>
        handler.GetType().FullName!.Split('.').Last();
}
