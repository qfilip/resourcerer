using FluentValidation;

namespace Resourcerer.Dtos;

public class DtoValidator
{
    public static string[] Validate<TDto, TValidator>(TDto dto)
        where TDto : class
        where TValidator : AbstractValidator<TDto>, new()
    {
        var validator = new TValidator();
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
