using FluentValidation;

namespace Resourcerer.Dtos;

public interface IBaseDto<T>
{
    AbstractValidator<T>? GetValidator();
}
