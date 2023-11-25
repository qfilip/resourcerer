namespace Resourcerer.DataAccess.Entities;

public class Instance : EntityBase
{
    public Instance()
    {
        InstanceSoldEvents = new HashSet<InstanceSoldEvent>();
        InstanceDiscardedEvents = new HashSet<InstanceDiscardedEvent>();
    }
    
    public DateTime? ExpiryDate { get; set; }

    public Guid? ItemId { get; set; }
    public virtual Item? Item { get; set; }

    public Guid InstanceBoughtEventId { get; set; }
    public virtual InstanceBoughtEvent? InstanceBoughtEvent { get; set; }
    
    public ICollection<InstanceSoldEvent> InstanceSoldEvents { get; set; }
    public ICollection<InstanceDiscardedEvent> InstanceDiscardedEvents { get; set; }    
}
