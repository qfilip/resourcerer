namespace Resourcerer.DataAccess.Entities;

public class InstanceRequestDeliveredEvent : EntityBase
{
    public Guid? InstanceBuyRequestedEventId { get; set; }
    public virtual InstanceBuyRequestedEvent? InstanceBuyRequestedEvent { get; set; }

    public Guid? InstanceSellRequestedEventId { get; set; }
    public virtual InstanceSellRequestedEvent? InstanceSellRequestedEvent { get; set; }
}
