namespace Resourcerer.Dtos;

public class ElementSoldEventDto : EntityDto
{
    public double UnitsSold { get; set; }
    public double PriceByUnit { get; set; }
    public int TotalDiscountPercent { get; set; }

    public UnitOfMeasureDto? UnitOfMeasure { get; set; }

    public Guid ElementId { get; set; }
    public ElementDto? Element { get; set; }
}
