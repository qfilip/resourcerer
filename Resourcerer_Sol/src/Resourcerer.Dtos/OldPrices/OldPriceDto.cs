using FluentValidation;
using Resourcerer.Dtos.Composites;
using Resourcerer.Dtos.Elements;

namespace Resourcerer.Dtos.OldPrices;

public class OldPriceDto : EntityDto
{
    public double UnitValue { get; set; }

    public Guid? ElementId { get; set; }
    public ElementDto? Element { get; set; }

    public Guid? CompositeId { get; set; }
    public virtual CompositeDto? Composite { get; set; }
}

public class PriceDtoValidator : AbstractValidator<OldPriceDto>
{
    public PriceDtoValidator()
    {
        RuleFor(x => x.CompositeId)
            .NotEmpty().WithMessage("CompositeId cannot be emmpty value");

        RuleFor(x => x.UnitValue)
            .GreaterThan(0).WithMessage("Price value must be bigger than 0");
    }
}
