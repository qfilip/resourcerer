namespace Resourcerer.Dtos.Instances.Events.Order;

public class InstanceOrderRequestDto : InstanceOrderEventDtoBase
{
    public Guid InstanceId { get; set; }
    public Guid SellerCompanyId { get; set; }
    public Guid BuyerCompanyId { get; set; }
    public Guid? DerivedInstanceItemId { get; set; }
    public double UnitsOrdered { get; set; }
    public double UnitPrice { get; set; }
    public int TotalDiscountPercent { get; set; }
    public DateTime? ExpectedDeliveryDate { get; set; }
}
