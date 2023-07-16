using FluentValidation;

namespace Resourcerer.Dtos;

public class InstanceOrderCancelledEventDto : BaseDto<InstanceOrderCancelledEventDto>
{
    public Guid InstanceOrderedEventId { get; set; }
    public override AbstractValidator<InstanceOrderCancelledEventDto> GetValidator() =>
        new Validator();

    private class Validator : AbstractValidator<InstanceOrderCancelledEventDto>
    {
        public Validator()
        {
            RuleFor(x => x.InstanceOrderedEventId)
                .NotEmpty()
                .WithMessage("Order event id cannot be empty");
        }
    }
}
