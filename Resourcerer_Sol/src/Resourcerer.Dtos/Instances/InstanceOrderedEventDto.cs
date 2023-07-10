using FluentValidation;

namespace Resourcerer.Dtos;

public class InstanceOrderedEventDto : EntityDto<InstanceOrderedEventDto>
{
    public Guid? ElementId { get; set; }
    public Guid? CompositeId { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public double UnitsOrdered { get; set; }
    public double UnitPrice { get; set; }
    public int TotalDiscountPercent { get; set; }
    public DateTime ExpectedDeliveryTime { get; set; }

    public override AbstractValidator<InstanceOrderedEventDto> GetValidator() =>
        new Validator();

    private class Validator : AbstractValidator<InstanceOrderedEventDto>
    {
        public Validator()
        {
            RuleFor(x => x.UnitsOrdered)
                .GreaterThan(0)
                .WithMessage("Number of ordered units must be greater than 0");

            RuleFor(x => x.UnitPrice)
                .GreaterThan(0)
                .WithMessage("Unit price must be greater than 0");

            RuleFor(x => x.TotalDiscountPercent)
                .InclusiveBetween(0, 100)
                .WithMessage("Total discount must be greater between in 0 and 100");
        }
    }
}
