using Resourcerer.Dtos.Events;

namespace Resourcerer.Dtos;

public class ItemOrderedEventDto : ItemEventDtoBase
{
    public Guid ItemId { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? Seller { get; set; }
    public string? Buyer { get; set; }
    public double UnitsOrdered { get; set; }
    public double UnitPrice { get; set; }
    public int TotalDiscountPercent { get; set; }
    public DateTime? ExpectedDeliveryDate { get; set; }
}
