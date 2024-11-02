using FluentValidation.Results;
using Resourcerer.Logic.Models;

namespace Resourcerer.Application.Logic.Handlers;

public interface IAppHandler<TRequest, TResponse>
{
    ValidationResult Validate(TRequest request);
    Task<HandlerResult<TResponse>> Handle(TRequest request);
}
