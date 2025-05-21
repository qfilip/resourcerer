using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.Entities;

namespace Resourcerer.Dtos;

public class InstanceOrderedEventDto : EntityDto
{
    public Guid DerivedInstanceId { get; set; }
    public Guid SellerCompanyId { get; set; }
    public Guid BuyerCompanyId { get; set; }
    public Guid? DerivedInstanceItemId { get; set; }
    public double Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public int TotalDiscountPercent { get; set; }
    public DateTime? ExpectedDeliveryDate { get; set; }

    // relational
    public Guid InstanceId { get; set; }
    public InstanceDto? Instance { get; set; }

    // json
    public InstanceOrderCancelledEvent? CancelledEvent { get; set; }
    public InstanceOrderSentEvent? SentEvent { get; set; }
    public InstanceOrderDeliveredEvent? DeliveredEvent { get; set; }
}
