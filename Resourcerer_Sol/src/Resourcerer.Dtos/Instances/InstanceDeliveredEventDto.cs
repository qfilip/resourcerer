using FluentValidation;

namespace Resourcerer.Dtos;

public class InstanceDeliveredEventDto : BaseDto<InstanceDeliveredEventDto>
{
    public Guid InstanceOrderedEventId { get; set; }
    public virtual InstanceOrderedEventDto? InstanceOrderedEvent { get; set; }

    public override AbstractValidator<InstanceDeliveredEventDto> GetValidator() =>
        new Validator();

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
