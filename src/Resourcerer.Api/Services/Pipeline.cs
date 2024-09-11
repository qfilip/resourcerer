using FluentValidation;
using Resourcerer.Application.Abstractions.Handlers;
using Resourcerer.Application.Models;
using Resourcerer.Messaging.Abstractions;

namespace Resourcerer.Api.Services;

public class Pipeline
{
    private readonly ILogger<Pipeline> _logger;

    public Pipeline(ILogger<Pipeline> logger)
    {
        _logger = logger;
    }

    public async Task<IResult> Pipe<TRequest, TResponse>(
        IAppHandler<TRequest, TResponse> handler,
        TRequest request,
        Func<TResponse, IResult>? customOkResultMapper = null)
    {
        var actionName = GetHandlerName(handler);

        _logger.LogInformation("Action {Action} started", actionName);

        var validationResult = handler.Validate(request);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(x => x.ErrorMessage);
            _logger.LogInformation("Action {Action} finished with validation errors", actionName);
            return Results.BadRequest(errors);
        }

        var handlerResult = await handler.Handle(request);

        _logger.LogInformation("Action {Action} finished", actionName);

        return MapResult(handlerResult, customOkResultMapper);
    }

    public async Task<IResult> PipeMessage<TRequest, TRequestBase>(
        TRequest request,
        IValidator<TRequest> validator,
        IMessageSender<TRequestBase> sender,
        string actionName) where TRequest : TRequestBase
    {
        _logger.LogInformation("Action {Action} started", actionName);

        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(x => x.ErrorMessage);
            _logger.LogInformation("Action {Action} finished with validation errors", actionName);
            
            return Results.BadRequest(errors);
        }

        await sender.SendAsync(request);

        _logger.LogInformation("Action {Action} finished", actionName);

        return Results.Accepted();
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
            (eHandlerResultStatus.Rejected, _) => Results.Conflict(handlerResult.Errors),
            _ =>
                throw new NotImplementedException($"Cannot handle status of {handlerResult.Status}")
        };
    }

    private string GetHandlerName(object handler) => handler.GetType().Name;
}
