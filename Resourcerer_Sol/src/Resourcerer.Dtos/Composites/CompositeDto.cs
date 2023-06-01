using Resourcerer.Dtos.Categories;
using Resourcerer.Dtos.CompositeSoldEvents;
using Resourcerer.Dtos.Excerpts;
using Resourcerer.Dtos.Prices;

namespace Resourcerer.Dtos.Composites;

public class CompositeDto : DtoBase
{
    public string? Name { get; set; }
    public Guid CategoryId { get; set; }

    public CategoryDto? Category { get; set; }
    public List<PriceDto> Prices { get; set; } = new();
    public List<ExcerptDto> Excerpts { get; set; } = new();
    public List<CompositeSoldEventDto> CompositeSoldEvents { get; set; } = new();
}
