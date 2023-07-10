namespace Resourcerer.DataAccess.Entities;

public class Instance : EntityBase
{
    public Instance()
    {
        InstanceOrderedEvents = new HashSet<InstanceOrderedEvent>();
        InstanceOrderCancelledEvents = new HashSet<InstanceOrderCancelledEvent>();
        InstanceDeliveredEvents = new HashSet<InstanceDeliveredEvent>();
        InstanceDiscardedEvents = new HashSet<InstanceDiscardedEvent>();
    }
    public double Quantity { get; set; }
    public DateTime? ExpiryDate { get; set; }

    public Guid? ElementId { get; set; }
    public virtual Element? Element { get; set; }
    public Guid? CompositeId { get; set; }
    public virtual Composite? Composite { get; set; }

    public ICollection<InstanceOrderedEvent> InstanceOrderedEvents { get; set; }
    public ICollection<InstanceOrderCancelledEvent> InstanceOrderCancelledEvents { get; set; }
    public ICollection<InstanceDeliveredEvent> InstanceDeliveredEvents { get; set; }
    public ICollection<InstanceDiscardedEvent> InstanceDiscardedEvents { get; set; }    
}
