using Resourcerer.Dtos.Entity;

namespace Resourcerer.Dtos;

public sealed class V1CreateElementItemFormDataDto : IDto
{
    public Guid CompanyId { get; set; }
    public CategoryDto[] Categories { get; set; } = Array.Empty<CategoryDto>();
    public UnitOfMeasureDto[] UnitsOfMeasure { get; set; } = Array.Empty<UnitOfMeasureDto>();
}
