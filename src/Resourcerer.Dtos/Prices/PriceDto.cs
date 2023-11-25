using FluentValidation;

namespace Resourcerer.Dtos;

public class PriceDto : IBaseDto
{
    public double UnitValue { get; set; }

    public Guid? ElementId { get; set; }
    public ItemDto? Element { get; set; }

    public class Validator : AbstractValidator<PriceDto>
    {
        public Validator()
        {
            RuleFor(x => x.UnitValue)
                .GreaterThan(0).WithMessage("Price value must be bigger than 0");
        }
    }
}


