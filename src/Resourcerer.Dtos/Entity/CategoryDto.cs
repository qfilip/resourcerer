namespace Resourcerer.Dtos.Entity;

public class CategoryDto : EntityDto<CategoryDto>
{
    public string? Name { get; set; }
    public Guid? ParentCategoryId { get; set; }

    public CategoryDto? ParentCategory { get; set; }
    public CategoryDto[] ChildCategories { get; set; } = Array.Empty<CategoryDto>();
    public ItemDto[] Items { get; set; } = Array.Empty<ItemDto>();


}
