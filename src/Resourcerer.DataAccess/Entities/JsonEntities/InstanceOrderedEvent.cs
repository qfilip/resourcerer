using Resourcerer.DataAccess.Entities.JsonEntities;

namespace Resourcerer.DataAccess.Entities;

public class InstanceOrderedEvent : JsonEntityBase
{
    public Guid SellerCompanyId { get; set; }
    public Guid BuyerCompanyId { get; set; }
    public double Quantity { get; set; }
    public double UnitPrice { get; set; }
    public int TotalDiscountPercent { get; set; }
    public DateTime? ExpectedDeliveryDate { get; set; }


    public virtual InstanceOrderCancelledEvent? InstanceOrderCancelledEvent { get; set; }
    public virtual InstanceSentEvent? InstanceSentEvent { get; set; }
    public virtual InstanceDeliveredEvent? InstanceDeliveredEvent { get; set; }
}
