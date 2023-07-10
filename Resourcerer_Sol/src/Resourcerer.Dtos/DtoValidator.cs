using FluentValidation;

namespace Resourcerer.Dtos;

public class DtoValidator
{
    public static string[] Validate<TDto>(TDto dto, AbstractValidator<TDto>? validator)
    {
        if (validator == null)
        {
            return Array.Empty<string>();
        }

        var validationResult = validator.Validate(dto);
        
        if(validationResult.IsValid)
        {
            return Array.Empty<string>();
        }
        else
        {
            return validationResult.Errors.Select(x => x.ErrorMessage).ToArray();
        }
    }
}
