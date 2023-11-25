using FluentValidation;

namespace Resourcerer.Dtos;

public class ItemOrderCancelledEventDto : IBaseDto
{
    public Guid InstanceOrderedEventId { get; set; }

    public class Validator : AbstractValidator<ItemOrderCancelledEventDto>
    {
        public Validator()
        {
            RuleFor(x => x.InstanceOrderedEventId)
                .NotEmpty()
                .WithMessage("Order event id cannot be empty");
        }
    }
}
