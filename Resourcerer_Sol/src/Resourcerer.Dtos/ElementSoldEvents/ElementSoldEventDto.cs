using Resourcerer.Dtos.Elements;
using Resourcerer.Dtos.UnitsOfMeasure;

namespace Resourcerer.Dtos.ElementSoldEvents;

public class ElementSoldEventDto : EntityDto
{
    public double UnitsSold { get; set; }
    public double PriceByUnit { get; set; }
    public double TotalDiscountPercent { get; set; }

    public UnitOfMeasureDto? UnitOfMeasure { get; set; }

    public Guid ElementId { get; set; }
    public ElementDto? Element { get; set; }
}
