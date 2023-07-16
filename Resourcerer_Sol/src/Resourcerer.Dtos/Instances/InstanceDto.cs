using FluentValidation;

namespace Resourcerer.Dtos;
public class InstanceDto : BaseDto<InstanceDto>
{
    public double Quantity { get; set; }
    public DateTime? ExpiryDate { get; set; }

    public Guid? ElementId { get; set; }
    public virtual ItemDto? Element { get; set; }

    public List<InstanceOrderedEventDto>? InstanceOrderedEvents { get; set; }
    public List<InstanceOrderCancelledEventDto>? InstanceOrderCancelledEvents { get; set; }
    public List<InstanceDeliveredEventDto>? InstanceDeliveredEvents { get; set; }
    public List<InstanceDiscardedEventDto>? InstanceDiscardedEvents { get; set; }


    public override AbstractValidator<InstanceDto> GetValidator() => new Validator();
    private class Validator : AbstractValidator<InstanceDto>
    {
        public Validator()
        {
            RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .WithMessage("Quantity must be larger than 0");
        }
    }
}
