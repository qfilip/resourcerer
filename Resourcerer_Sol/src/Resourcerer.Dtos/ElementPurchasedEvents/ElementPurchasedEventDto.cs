using Resourcerer.Dtos.Elements;

namespace Resourcerer.Dtos.ElementPurchasedEvents;

public class ElementPurchasedEventDto : EntityDto
{
    public Guid ElementId { get; set; }
    public double NumOfUnits { get; set; }
    public double UnitPrice { get; set; }

    public ElementDto? Element { get; set; }
}
