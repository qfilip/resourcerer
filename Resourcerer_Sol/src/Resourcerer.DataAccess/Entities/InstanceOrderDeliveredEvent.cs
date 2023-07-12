namespace Resourcerer.DataAccess.Entities;

public class InstanceOrderDeliveredEvent : EntityBase
{
    public Guid InstanceOrderedEventId { get; set; }
    public virtual InstanceOrderedEvent? InstanceOrderedEvent { get; set; }
}
