using Resourcerer.Dtos.Entities;
using Resourcerer.Dtos.Entities.Json;

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
    public InstanceOrderCancelledEventDto? CancelledEvent { get; set; }
    public InstanceOrderSentEventDto? SentEvent { get; set; }
    public InstanceOrderDeliveredEventDto? DeliveredEvent { get; set; }
}
