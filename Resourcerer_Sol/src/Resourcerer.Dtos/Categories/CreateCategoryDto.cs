using FluentValidation;

namespace Resourcerer.Dtos;

public class CreateCategoryDto : IBaseDto<CreateCategoryDto>
{
    public string? Name { get; set; }
    public Guid? ParentCategoryId { get; set; }

    public AbstractValidator<CreateCategoryDto>? GetValidator() => null;
}

