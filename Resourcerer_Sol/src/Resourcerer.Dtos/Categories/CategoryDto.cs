using Resourcerer.Dtos.Composites;
using Resourcerer.Dtos.Elements;

namespace Resourcerer.Dtos.Categories;

public class CategoryDto : DtoBase
{
    public string? Name { get; set; }
    public Guid? ParentCategoryId { get; set; }

    public virtual CategoryDto? ParentCategory { get; set; }
    public List<CategoryDto> ChildCategories { get; set; } = new();
    public List<CompositeDto> Composites { get; set; } = new();
    public List<ElementDto> Elements { get; set; } = new();
}
