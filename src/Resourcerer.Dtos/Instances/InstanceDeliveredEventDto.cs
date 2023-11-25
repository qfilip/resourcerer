using FluentValidation;

namespace Resourcerer.Dtos;

public class InstanceDeliveredEventDto : IBaseDto
{
    public Guid InstanceOrderedEventId { get; set; }
    public virtual InstanceOrderedEventDto? InstanceOrderedEvent { get; set; }

    public class Validator : AbstractValidator<InstanceDeliveredEventDto>
    {
        public Validator()
        {
            RuleFor(x => x.InstanceOrderedEventId)
                .NotEmpty()
                .WithMessage("InstanceOrderedEventId cannot be empty");
        }
    }
}
