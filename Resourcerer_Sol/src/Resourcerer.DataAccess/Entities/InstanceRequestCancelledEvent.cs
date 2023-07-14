namespace Resourcerer.DataAccess.Entities;

public class InstanceRequestCancelledEvent : EntityBase
{
    public string? Reason { get; set; }
    
    public Guid? InstanceBuyRequestedEventId { get; set; }
    public virtual InstanceBuyRequestedEvent? InstanceBuyRequestedEvent { get; set; }

    public Guid? InstanceSellRequestedEventId { get; set; }
    public virtual InstanceBuyRequestedEvent? InstanceSellRequestedEvent { get; set; }
}
