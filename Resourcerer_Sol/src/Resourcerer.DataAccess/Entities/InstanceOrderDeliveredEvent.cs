namespace Resourcerer.DataAccess.Entities;

public class InstanceOrderDeliveredEvent : EntityBase
{
    public Guid InstanceOrderedEventId { get; set; }
    public virtual InstanceOrderedEvent? InstanceOrderedEvent { get; set; }

    public Guid? InstanceOrderCancelledEventId { get; set; }
    public virtual InstanceOrderCancelledEvent? InstanceOrderCancelledEvent { get; set; }
}
