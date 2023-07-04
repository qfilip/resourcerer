using FluentValidation;

namespace Resourcerer.Dtos;

public class CategoryDto : EntityDto
{
    public string? Name { get; set; }
    public Guid? ParentCategoryId { get; set; }

    public CategoryDto? ParentCategory { get; set; }
    public List<CategoryDto> ChildCategories { get; set; } = new();
    public List<CompositeDto> Composites { get; set; } = new();
    public List<ElementDto> Elements { get; set; } = new();
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