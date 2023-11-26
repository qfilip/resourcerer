using FluentValidation;

namespace Resourcerer.Dtos;

public class ItemDeliveredEventDto : IBaseDto
{
    public Guid InstanceOrderedEventId { get; set; }
    public virtual ItemOrderedEventDto? InstanceOrderedEvent { get; set; }

    public class Validator : AbstractValidator<ItemDeliveredEventDto>
    {
        public Validator()
        {
            RuleFor(x => x.InstanceOrderedEventId)
                .NotEmpty()
                .WithMessage("InstanceOrderedEventId cannot be empty");
        }
    }
}
