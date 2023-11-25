namespace Resourcerer.DataAccess.Entities;

public class ItemSellCancelledEvent : EntityBase
{
    public string? Reason { get; set; }
    public double RefundedAmount { get; set; }
    
    public Guid? InstanceBoughtEventId { get; set; }
    public virtual ItemOrderedEvent? InstanceBoughtEvent { get; set; }

    public Guid? InstanceSoldEventId { get; set; }
    public virtual ItemSoldEvent? InstanceSoldEvent { get; set; }
}
