using Resourcerer.Dtos.Composites;
using Resourcerer.Dtos.CompositeSoldEvents;

namespace Resourcerer.Dtos.Prices;

public class PriceDto : DtoBase
{
    public Guid CompositeId { get; set; }
    public double Value { get; set; }

    public CompositeDto? Composite { get; set; }
    public List<CompositeSoldEventDto> CompositeSoldEvents { get; set; } = new();
}
