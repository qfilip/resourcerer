using FluentValidation;

namespace Resourcerer.Dtos;

public class PriceDto : BaseDto<PriceDto>
{
    public double UnitValue { get; set; }

    public Guid? ElementId { get; set; }
    public ItemDto? Element { get; set; }

    public override AbstractValidator<PriceDto> GetValidator() => new Validator();

    public class Validator : AbstractValidator<PriceDto>
    {
        public Validator()
        {
            RuleFor(x => x.UnitValue)
                .GreaterThan(0).WithMessage("Price value must be bigger than 0");
        }
    }
}


