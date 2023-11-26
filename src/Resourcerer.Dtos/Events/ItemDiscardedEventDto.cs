using FluentValidation;
using Resourcerer.Dtos.Events;

namespace Resourcerer.Dtos;

public class ItemDiscardedEventDto : ItemEventDtoBase
{
    public double Quantity { get; set; }
    public string? Reason { get; set; }

    public Guid InstanceId { get; set; }
    public virtual InstanceDto? Instance { get; set; }

    public class Validator : AbstractValidator<ItemDiscardedEventDto>
    {
        public Validator()
        {
            RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .WithMessage("Quantity must be larger than 0");

            RuleFor(x => x.Reason)
                .NotEmpty()
                .WithMessage("Reason cannot be empty");

            RuleFor(x => x.InstanceId)
                .NotEmpty()
                .WithMessage("InstanceId cannot be empty");
        }
    }
}
