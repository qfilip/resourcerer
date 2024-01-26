namespace Resourcerer.DataAccess.Entities;

public class ItemOrderCancelledEvent : EntityBase
{
    public string? Reason { get; set; }
    public double RefundedAmount { get; set; }
    
    public Guid? ItemOrderedEventId { get; set; }
    public virtual ItemOrderedEvent? ItemOrderedEvent { get; set; }
}
