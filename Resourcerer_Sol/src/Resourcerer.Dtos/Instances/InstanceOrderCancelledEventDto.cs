using FluentValidation;

namespace Resourcerer.Dtos;

public class InstanceOrderCancelledEventDto : EntityDto<InstanceOrderCancelledEventDto>
{
    public override AbstractValidator<InstanceOrderCancelledEventDto> GetValidator() =>
        new Validator();

    private class Validator : AbstractValidator<InstanceOrderCancelledEventDto>
    {
        public Validator()
        {
            
        }
    }
}
