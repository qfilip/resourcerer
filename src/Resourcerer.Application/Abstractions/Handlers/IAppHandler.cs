using FluentValidation.Results;
using Resourcerer.Application.Models;

namespace Resourcerer.Application.Abstractions.Handlers;

public interface IAppHandler<TRequest, TResponse>
{
    ValidationResult Validate(TRequest request);
    Task<HandlerResult<TResponse>> Handle(TRequest request);
}
