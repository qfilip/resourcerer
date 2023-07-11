using FluentValidation;

namespace Resourcerer.Dtos;

public class CreateCompositeItemDto : IBaseDto<CreateCompositeItemDto>
{
    public AbstractValidator<CreateCompositeItemDto>? GetValidator() => new Validator();

    private class Validator : AbstractValidator<CreateCompositeItemDto>
    {
        public Validator()
        {
            
        }
    }
}
