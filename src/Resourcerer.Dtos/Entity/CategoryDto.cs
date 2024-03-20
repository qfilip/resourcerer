namespace Resourcerer.Dtos.Entity;

public class CategoryDto : EntityDto<CategoryDto>
{
    public string? Name { get; set; }
    public Guid? ParentCategoryId { get; set; }

    public CategoryDto? ParentCategory { get; set; }
    public List<CategoryDto> ChildCategories { get; set; } = new();
    public List<ItemDto> Elements { get; set; } = new();


}
