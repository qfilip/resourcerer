namespace Resourcerer.DataAccess.Entities;

public class Instance : EntityBase
{
    public Instance()
    {
        InstanceDiscardedEvents = new HashSet<InstanceDiscardedEvent>();
    }
    public double UnitsOrdered { get; set; }
    public double UnitPrice { get; set; }
    public int TotalDiscountPercent { get; set; }
    public DateTime? ExpectedDeliveryDate { get; set; }
    public DateTime? ExpiryDate { get; set; }

    public Guid? ItemId { get; set; }
    public virtual Item? Item { get; set; }

    public Guid InstanceOrderedEventId { get; set; }
    public virtual InstanceOrderedEvent? InstanceOrderedEvent { get; set; }
    public ICollection<InstanceDiscardedEvent> InstanceDiscardedEvents { get; set; }    
}
