using FluentValidation;

namespace Resourcerer.Dtos;
public class InstanceDto : IBaseDto
{
    public double Quantity { get; set; }
    public DateTime? ExpiryDate { get; set; }

    public Guid? ElementId { get; set; }
    public virtual ItemDto? Element { get; set; }

    public List<InstanceOrderRequestDto>? InstanceOrderedEvents { get; set; }
    public List<InstanceCancelRequestDto>? InstanceOrderCancelledEvents { get; set; }
    public List<ItemDeliveredEventDto>? InstanceDeliveredEvents { get; set; }
    public List<ItemDiscardedEventDto>? InstanceDiscardedEvents { get; set; }

    public class Validator : AbstractValidator<InstanceDto>
    {
        public Validator()
        {
            RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .WithMessage("Quantity must be larger than 0");
        }
    }
}
