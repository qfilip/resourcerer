namespace Resourcerer.Dtos.Categories;

public class CreateCategoryDto : BaseDto
{
    public string? Name { get; set; }
    public Guid? ParentCategoryId { get; set; }
}

