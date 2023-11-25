using FluentValidation;

namespace Resourcerer.Dtos;

public class InstanceOrderCancelledEventDto : IBaseDto
{
    public Guid InstanceOrderedEventId { get; set; }

    public class Validator : AbstractValidator<InstanceOrderCancelledEventDto>
    {
        public Validator()
        {
            RuleFor(x => x.InstanceOrderedEventId)
                .NotEmpty()
                .WithMessage("Order event id cannot be empty");
        }
    }
}
