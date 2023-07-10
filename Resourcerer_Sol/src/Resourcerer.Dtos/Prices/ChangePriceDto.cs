using FluentValidation;

namespace Resourcerer.Dtos;

public class ChangePriceDto : IBaseDto<ChangePriceDto>
{
    public Guid EntityId { get; set; }
    public double UnitPrice { get; set; }

    public AbstractValidator<ChangePriceDto>? GetValidator() => new Validator();

    private class Validator : AbstractValidator<ChangePriceDto>
    {
        public Validator()
        {
            RuleFor(x => x.EntityId)
                .NotEmpty().WithMessage("Entity id cannot be default value");

            RuleFor(x => x.UnitPrice)
                .GreaterThan(0).WithMessage("Entity price must be greater than 0");
        }
    }
}


