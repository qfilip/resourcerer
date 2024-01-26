namespace Resourcerer.DataAccess.Entities;

public class Instance : EntityBase
{
    public Instance()
    {
        ItemOrderedEvents = new HashSet<ItemOrderedEvent>();
        ItemDiscardedEvents = new HashSet<ItemDiscardedEvent>();
    }
    
    public DateTime? ExpiryDate { get; set; }

    public Guid? ItemId { get; set; }
    public virtual Item? Item { get; set; }

    public Guid ItemOrderedEventId { get; set; }
    public ICollection<ItemOrderedEvent> ItemOrderedEvents { get; set; }
    public ICollection<ItemDiscardedEvent> ItemDiscardedEvents { get; set; }    
}
