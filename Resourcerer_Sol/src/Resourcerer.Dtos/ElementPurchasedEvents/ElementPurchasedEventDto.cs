using Resourcerer.Dtos.Elements;

namespace Resourcerer.Dtos.ElementPurchasedEvents;

public class ElementPurchasedEventDto : DtoBase
{
    public Guid ElementId { get; set; }
    public int NumOfUnits { get; set; }
    public double UnitPrice { get; set; }

    public virtual ElementDto? Element { get; set; }
}
