namespace Resourcerer.DataAccess.Entities;

public class InstanceOrderCancelledEvent : EntityBase
{
    public Guid InstanceOrderedEventId { get; set; }
    public virtual InstanceOrderedEvent? InstanceOrderedEvent { get; set; }

    public Guid? InstanceDeliveredEventId { get; set; }
    public virtual InstanceOrderDeliveredEvent? InstanceDeliveredEvent { get; set; }
}
