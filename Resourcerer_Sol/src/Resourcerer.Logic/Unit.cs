using FluentValidation;
using Resourcerer.Dtos;

namespace Resourcerer.Logic;

public struct Unit : IBaseDto<Unit>
{
    public AbstractValidator<Unit>? GetValidator() => null;
}
