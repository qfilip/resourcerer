namespace Resourcerer.DataAccess.Entities;

public class Instance : EntityBase
{
    public Instance()
    {
        ItemDiscardedEvents = new HashSet<ItemDiscardedEvent>();
    }
    
    public DateTime? ExpiryDate { get; set; }

    public Guid? ItemId { get; set; }
    public virtual Item? Item { get; set; }

    public Guid ItemOrderedEventId { get; set; }
    public virtual ItemOrderedEvent? ItemOrderedEvent { get; set; }
    public ICollection<ItemDiscardedEvent> ItemDiscardedEvents { get; set; }    
}
