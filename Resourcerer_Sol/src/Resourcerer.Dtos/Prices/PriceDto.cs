using FluentValidation;
using Resourcerer.Dtos.Composites;

namespace Resourcerer.Dtos.Prices;

public class PriceDto : EntityDto
{
    public Guid CompositeId { get; set; }
    public double Value { get; set; }

    public CompositeDto? Composite { get; set; }
}

public class PriceDtoValidator : AbstractValidator<PriceDto>
{
    public PriceDtoValidator()
    {
        RuleFor(x => x.CompositeId)
            .NotEmpty().WithMessage("CompositeId cannot be emmpty value");

        RuleFor(x => x.Value)
            .GreaterThan(0).WithMessage("Price value must be bigger than 0");
    }
}
