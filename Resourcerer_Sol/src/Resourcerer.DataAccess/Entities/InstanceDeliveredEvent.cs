namespace Resourcerer.DataAccess.Entities;

public class InstanceDeliveredEvent : EntityBase
{
    public Guid? InstanceBuyRequestedEventId { get; set; }
    public virtual InstanceBoughtEvent? InstanceBuyRequestedEvent { get; set; }

    public Guid? InstanceSellRequestedEventId { get; set; }
    public virtual InstanceSoldEvent? InstanceSellRequestedEvent { get; set; }
}
