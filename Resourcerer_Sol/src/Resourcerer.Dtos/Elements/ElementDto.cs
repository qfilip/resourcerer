using Resourcerer.Dtos.Categories;
using Resourcerer.Dtos.ElementPurchasedEvents;
using Resourcerer.Dtos.Excerpts;
using Resourcerer.Dtos.UnitsOfMeasure;

namespace Resourcerer.Dtos.Elements;

public class ElementDto : EntityDto
{
    public string? Name { get; set; }
    public Guid CategoryId { get; set; }
    public Guid UnitOfMeasureId { get; set; }

    public CategoryDto? Category { get; set; }
    public UnitOfMeasureDto? UnitOfMeasure { get; set; }
    public List<ExcerptDto> Excerpts { get; set; } = new();
    public List<ElementPurchasedEventDto> ElementPurchasedEvents { get; set; } = new();
}
