using FluentValidation;

namespace Resourcerer.Dtos;

public class InstanceDeliveredEventDto : EntityDto<InstanceDeliveredEventDto>
{
    public Guid InstanceOrderedEventId { get; set; }
    public virtual InstanceOrderedEventDto? InstanceOrderedEvent { get; set; }

    public override AbstractValidator<InstanceDeliveredEventDto> GetValidator() =>
        new InstanceDeliveredEventDtoValidator();
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
