namespace Resourcerer.DataAccess.Entities;

public class ItemOrderedEvent : EntityBase
{
    public string? Seller { get; set; }
    public string? Buyer { get; set; }
    public double Quantity { get; set; }
    public double UnitPrice { get; set; }
    public int TotalDiscountPercent { get; set; }
    public DateTime? ExpectedDeliveryDate { get; set; }

    public Guid InstanceId { get; set; }
    public virtual Instance? Instance { get; set; }

    public Guid? ItemOrderCancelledEventId { get; set; }
    public virtual ItemOrderCancelledEvent? ItemOrderCancelledEvent { get; set; }

    public Guid? ItemSentEventId { get; set; }
    public virtual ItemSentEvent? ItemSentEvent { get; set; }

    public Guid? ItemDeliveredEventId { get; set; }
    public virtual ItemDeliveredEvent? ItemDeliveredEvent { get; set; }
}
