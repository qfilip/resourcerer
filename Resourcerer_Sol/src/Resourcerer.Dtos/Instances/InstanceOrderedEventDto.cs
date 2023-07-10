using FluentValidation;

namespace Resourcerer.Dtos;

public class InstanceOrderedEventDto : EntityDto<InstanceOrderedEventDto>
{
    public override AbstractValidator<InstanceOrderedEventDto> GetValidator() =>
        new Validator();

    private class Validator : AbstractValidator<InstanceOrderedEventDto>
    {
        public Validator()
        {
        }
    }
}
