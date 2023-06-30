using Resourcerer.Dtos.Composites;

namespace Resourcerer.Dtos.CompositeSoldEvents;

public class CompositeSoldEventDto : EntityDto
{
    public double UnitsSold { get; set; }
    public double PriceByUnit { get; set; }

    public Guid CompositeId { get; set; }
    public CompositeDto? Composite { get; set; }
}
