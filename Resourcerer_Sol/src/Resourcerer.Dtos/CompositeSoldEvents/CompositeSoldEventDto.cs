using Resourcerer.Dtos.Composites;
using Resourcerer.Dtos.Prices;

namespace Resourcerer.Dtos.CompositeSoldEvents;

public class CompositeSoldEventDto : DtoBase
{
    public Guid CompositeId { get; set; }
    public Guid PriceId { get; set; }

    public CompositeDto? Composite { get; set; }
    public PriceDto? Price { get; set; }
}
