namespace Resourcerer.DataAccess.Entities;

public class InstanceOrderedEvent : EntityBase
{
    public string? Seller { get; set; }
    public string? Buyer { get; set; }
    public double Quantity { get; set; }
    public double UnitPrice { get; set; }
    public int TotalDiscountPercent { get; set; }
    public DateTime? ExpectedDeliveryDate { get; set; }


    public virtual InstanceOrderCancelledEvent? InstanceOrderCancelledEvent { get; set; }
    public virtual InstanceSentEvent? InstanceSentEvent { get; set; }
    public virtual InstanceDeliveredEvent? InstanceDeliveredEvent { get; set; }
}
