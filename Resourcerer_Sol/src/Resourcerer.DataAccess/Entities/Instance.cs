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
    public double UnitsOrdered { get; set; }
    public double UnitPrice { get; set; }
    public int TotalDiscountPercent { get; set; }
    public DateTime? ExpectedDeliveryDate { get; set; }
    public DateTime? ExpiryDate { get; set; }

    public Guid? ItemId { get; set; }
    public virtual Item? Item { get; set; }
    

    public ICollection<InstanceOrderedEvent> InstanceOrderedEvents { get; set; }
    public ICollection<InstanceOrderCancelledEvent> InstanceOrderCancelledEvents { get; set; }
    public ICollection<InstanceDeliveredEvent> InstanceDeliveredEvents { get; set; }
    public ICollection<InstanceDiscardedEvent> InstanceDiscardedEvents { get; set; }    
}
