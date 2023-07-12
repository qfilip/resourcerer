namespace Resourcerer.DataAccess.Entities;

public class InstanceOrderCancelledEvent : EntityBase
{
    public Guid InstanceOrderedEventId { get; set; }
    public string? Reason { get; set; }
    public virtual InstanceOrderedEvent? InstanceOrderedEvent { get; set; }
}
