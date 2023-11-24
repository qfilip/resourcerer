namespace Resourcerer.DataAccess.Entities;

public class InstanceCancelledEvent : EntityBase
{
    public string? Reason { get; set; }
    
    public Guid? InstanceBuyRequestedEventId { get; set; }
    public virtual InstanceBoughtEvent? InstanceBuyRequestedEvent { get; set; }

    public Guid? InstanceSellRequestedEventId { get; set; }
    public virtual InstanceSoldEvent? InstanceSellRequestedEvent { get; set; }
}
