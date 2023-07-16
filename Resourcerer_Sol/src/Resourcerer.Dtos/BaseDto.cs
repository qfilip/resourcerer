using FluentValidation;

namespace Resourcerer.Dtos;

public abstract class BaseDto<T> : IBaseDto
{
    public abstract AbstractValidator<T>? GetValidator();
}
