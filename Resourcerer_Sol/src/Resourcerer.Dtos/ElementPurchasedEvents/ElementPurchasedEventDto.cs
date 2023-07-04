namespace Resourcerer.Dtos;

public class ElementPurchasedEventDto : EntityDto
{
    public double UnitsBought { get; set; }
    public double UnitPrice { get; set; }
    public int TotalDiscountPercent { get; set; }
    public DateTime ExpectedDeliveryTime { get; set; }

    public Guid ElementId { get; set; }
    public ElementDto? Element { get; set; }
}
