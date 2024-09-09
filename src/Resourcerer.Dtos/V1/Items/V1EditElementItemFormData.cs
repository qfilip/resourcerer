using Resourcerer.Dtos.Entity;

namespace Resourcerer.Dtos.V1;

public sealed class V1EditElementItemFormData : IDto
{
    public ItemDto Item { get; set; } = new();
    public CategoryDto[] Categories { get; set; } = Array.Empty<CategoryDto>();
    public UnitOfMeasureDto[] UnitsOfMeasure { get; set; } = Array.Empty<UnitOfMeasureDto>();
}
