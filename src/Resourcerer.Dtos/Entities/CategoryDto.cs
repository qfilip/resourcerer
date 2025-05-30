﻿namespace Resourcerer.Dtos.Entities;

public class CategoryDto : EntityDto
{
    public string? Name { get; set; }

    public Guid CompanyId { get; set; }
    public virtual CompanyDto? Company { get; set; }

    public Guid? ParentCategoryId { get; set; }
    public virtual CategoryDto? ParentCategory { get; set; }

    public CategoryDto[] ChildCategories { get; set; } = Array.Empty<CategoryDto>();
    public ItemDto[] Items { get; set; } = Array.Empty<ItemDto>();
}
