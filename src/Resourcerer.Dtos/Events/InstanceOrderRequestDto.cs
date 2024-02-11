using Resourcerer.Dtos.Events;

namespace Resourcerer.Dtos;

public class InstanceOrderRequestDto : InstanceEventDtoBase
{
    public Guid InstanceId { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public Guid SellerCompanyId { get; set; }
    public Guid BuyerCompanyId { get; set; }
    public double UnitsOrdered { get; set; }
    public double UnitPrice { get; set; }
    public int TotalDiscountPercent { get; set; }
    public DateTime? ExpectedDeliveryDate { get; set; }
}
