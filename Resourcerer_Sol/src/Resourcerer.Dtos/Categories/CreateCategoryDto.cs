namespace Resourcerer.Dtos.Categories;

public class CreateCategoryDto : DtoBase
{
    public string? Name { get; set; }
    public Guid? ParentCategoryId { get; set; }
}

