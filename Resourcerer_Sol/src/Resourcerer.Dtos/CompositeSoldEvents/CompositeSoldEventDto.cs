namespace Resourcerer.Dtos;

public class CompositeSoldEventDto : EntityDto
{
    public double UnitsSold { get; set; }
    public double UnitPrice { get; set; }
    public int TotalDiscountPercent { get; set; }

    public Guid CompositeId { get; set; }
    public CompositeDto? Composite { get; set; }
}
