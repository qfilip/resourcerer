namespace Resourcerer.DataAccess.Entities;

public class Instance : EntityBase
{
    public Instance()
    {
        InstanceSoldEvents = new HashSet<ItemSoldEvent>();
        InstanceDiscardedEvents = new HashSet<ItemDiscardedEvent>();
    }
    
    public DateTime? ExpiryDate { get; set; }

    public Guid? ItemId { get; set; }
    public virtual Item? Item { get; set; }

    public Guid InstanceBoughtEventId { get; set; }
    public virtual ItemOrderedEvent? InstanceBoughtEvent { get; set; }
    
    public ICollection<ItemSoldEvent> InstanceSoldEvents { get; set; }
    public ICollection<ItemDiscardedEvent> InstanceDiscardedEvents { get; set; }    
}
