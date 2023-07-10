namespace Resourcerer.DataAccess.Entities;

public class InstanceDeliveredEvent : EntityBase
{
    public Guid? InstanceOrderedEventId { get; set; }
    public virtual InstanceOrderedEvent? InstanceOrderedEvent { get; set; }
}
