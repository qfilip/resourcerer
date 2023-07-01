using Resourcerer.Dtos.Elements;

namespace Resourcerer.Dtos.ElementPurchasedEvents;

public class ElementPurchasedEventDto : EntityDto
{
    public double UnitsBought { get; set; }
    public double UnitPrice { get; set; }
    public double TotalDiscountPercent { get; set; }

    public Guid ElementId { get; set; }
    public ElementDto? Element { get; set; }
}
