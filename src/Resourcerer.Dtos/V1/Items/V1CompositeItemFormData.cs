using Resourcerer.Dtos.Entities;

namespace Resourcerer.Dtos.V1;

public class V1CompositeItemFormData : IDto
{
    public ItemDto[] Items { get; set; } = Array.Empty<ItemDto>();
    public CategoryDto[] Categories { get; set; } = Array.Empty<CategoryDto>();
    public UnitOfMeasureDto[] UnitsOfMeasure { get; set; } = Array.Empty<UnitOfMeasureDto>();
}
