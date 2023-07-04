using FluentValidation;

namespace Resourcerer.Dtos;

public class CreateElementPurchaseDto : BaseDto
{
    public Guid ElementId { get; set; }
    public double UnitsBought { get; set; }
    public double UnitPrice { get; set; }
    public int TotalDiscountPercent { get; set; }
}

public class CreateElementPurchaseDtoValidator: AbstractValidator<CreateElementPurchaseDto>
{
    public CreateElementPurchaseDtoValidator()
    {
        RuleFor(x => x.ElementId)
            .NotEmpty().WithMessage("Element id cannot be empty");

        RuleFor(x => x.UnitsBought)
            .GreaterThan(0).WithMessage("Units bought must greater than 0");

        RuleFor(x => x.UnitPrice)
            .GreaterThan(0).WithMessage("Units price must greater than 0");

        RuleFor(x => x.TotalDiscountPercent)
            .InclusiveBetween(0, 100).WithMessage("Discount percent must be between 0 and 100");
    }
}
