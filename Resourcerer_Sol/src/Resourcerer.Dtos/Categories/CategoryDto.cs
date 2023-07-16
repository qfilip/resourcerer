using FluentValidation;

namespace Resourcerer.Dtos;

public class CategoryDto : BaseDto<CategoryDto>
{
    public string? Name { get; set; }
    public Guid? ParentCategoryId { get; set; }

    public CategoryDto? ParentCategory { get; set; }
    public List<CategoryDto> ChildCategories { get; set; } = new();
    public List<ItemDto> Elements { get; set; } = new();

    public override AbstractValidator<CategoryDto> GetValidator() => new CategoryDtoValidator();
}

public class CategoryDtoValidator : AbstractValidator<CategoryDto>
{
    public CategoryDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Category name cannot be empty")
            .Length(min: 3, max: 50).WithMessage("Category name must be between 3 and 50 characters long");
    }
}