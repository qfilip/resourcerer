using FluentValidation;

namespace Resourcerer.Dtos;

public class InstanceDeliveredEventDto : EntityDto
{
    public Guid InstanceOrderedEventId { get; set; }
    public virtual InstanceOrderedEventDto? InstanceOrderedEvent { get; set; }
}

public class InstanceDeliveredEventDtoValidator : AbstractValidator<InstanceDeliveredEventDto>
{
    public InstanceDeliveredEventDtoValidator()
    {
        RuleFor(x => x.InstanceOrderedEventId)
            .NotEmpty()
            .WithMessage("InstanceOrderedEventId cannot be empty");
    }
}
