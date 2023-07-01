using FluentValidation;

namespace Resourcerer.Dtos.OldPrices;

public class ChangePriceDto : BaseDto
{
    public Guid EntityId { get; set; }
    public double NewPrice { get; set; }
}

public class ChangePriceDtoValidator : AbstractValidator<ChangePriceDto>
{
    public ChangePriceDtoValidator()
    {
        RuleFor(x => x.EntityId)
            .NotEmpty().WithMessage("Entity id cannot be default value");

        RuleFor(x => x.NewPrice)
            .GreaterThan(0).WithMessage("Entity price must be greater than 0");
    }
}
