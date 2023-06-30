using FluentValidation;
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
        Func<HandlerResult<TResponse>, IResult>? customResultMapper = null)
    {
        var actionName = handler.GetType().FullName;

        _logger.LogInformation("Action {Action} started", actionName);
        
        var handlerResult = await handler.Handle(request);
        
        _logger.LogInformation("Action {Action} finished", actionName);

        return MapResult(handlerResult, customResultMapper);
    }

    public async Task<IResult> Pipe<TRequest, TRequestValidator, TResponse>(
        IRequestHandler<TRequest, TResponse> handler,
        TRequest request,
        Func<HandlerResult<TResponse>, IResult>? customResultMapper = null)
        where TRequest : class
        where TRequestValidator : AbstractValidator<TRequest>, new()
    {
        var actionName = handler.GetType().FullName;

        _logger.LogInformation("Action {Action} started", actionName);
        
        var validationErrors = DtoValidator.Validate<TRequest, TRequestValidator>(request);
        if(validationErrors.Any())
        {
            _logger.LogInformation("Action {Action} finished with validation errors", actionName);
            return Results.BadRequest(validationErrors);
        }
        
        var handlerResult = await handler.Handle(request);
        _logger.LogInformation("Action {Action} finished", actionName);

        return MapResult(handlerResult, customResultMapper);
    }

    private static IResult MapResult<TResponse>(
        HandlerResult<TResponse> handlerResult,
        Func<HandlerResult<TResponse>, IResult>? customResultMapper = null)
    {
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
