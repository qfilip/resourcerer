using FluentValidation;

namespace Resourcerer.Dtos;

public class ChangePriceDto : IBaseDto
{
    public Guid ItemId { get; set; }
    public double UnitPrice { get; set; }

    public class Validator : AbstractValidator<ChangePriceDto>
    {
        public Validator()
        {
            RuleFor(x => x.ItemId)
                .NotEmpty().WithMessage("Item id cannot be default value");

            RuleFor(x => x.UnitPrice)
                .GreaterThan(0).WithMessage("Item price must be greater than 0");
        }
    }
}


