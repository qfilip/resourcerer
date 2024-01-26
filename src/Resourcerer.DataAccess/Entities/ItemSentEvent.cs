namespace Resourcerer.DataAccess.Entities;

public class ItemSentEvent : EntityBase
{
    public Guid? ItemOrderedEventId { get; set; }
    public virtual ItemOrderedEvent? ItemOrderedEvent { get; set; }
}
