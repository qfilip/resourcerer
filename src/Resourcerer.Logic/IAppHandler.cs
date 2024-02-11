using FluentValidation.Results;

namespace Resourcerer.Logic;

public interface IAppHandler<TRequest, TResponse>
{
    ValidationResult Validate(TRequest request);
    Task<HandlerResult<TResponse>> Handle(TRequest request);
}