using FluentValidation;
using Resourcerer.Dtos.Events;

namespace Resourcerer.Dtos;

public class ItemCancelledEventDto : ItemEventDtoBase
{
    public Guid InstanceOrderedEventId { get; set; }

    public class Validator : AbstractValidator<ItemCancelledEventDto>
    {
        public Validator()
        {
            RuleFor(x => x.InstanceOrderedEventId)
                .NotEmpty()
                .WithMessage("Order event id cannot be empty");
        }
    }
}
