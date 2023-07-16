using FluentValidation;

namespace Resourcerer.Dtos;

public class InstanceDiscardedEventDto : BaseDto<InstanceDiscardedEventDto>
{
    public double Quantity { get; set; }
    public string? Reason { get; set; }

    public Guid InstanceId { get; set; }
    public virtual InstanceDto? Instance { get; set; }

    public override AbstractValidator<InstanceDiscardedEventDto> GetValidator() =>
        new Validator();

    private class Validator : AbstractValidator<InstanceDiscardedEventDto>
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
