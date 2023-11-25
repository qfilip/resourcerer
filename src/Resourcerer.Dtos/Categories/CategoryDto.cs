using FluentValidation;

namespace Resourcerer.Dtos;

public class CategoryDto : EntityDto<CategoryDto>
{
    public string? Name { get; set; }
    public Guid? ParentCategoryId { get; set; }

    public CategoryDto? ParentCategory { get; set; }
    public List<CategoryDto> ChildCategories { get; set; } = new();
    public List<ItemDto> Elements { get; set; } = new();

    public class Validator : AbstractValidator<CategoryDto>
    {
        public Validator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Category name cannot be empty")
                .Length(min: 3, max: 50).WithMessage("Category name must be between 3 and 50 characters long");
        }
    }
}
