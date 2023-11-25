namespace Resourcerer.DataAccess.Entities;

public class ItemDeliveredEvent : EntityBase
{
    public Guid? ItemOrderedEventId { get; set; }
    public virtual ItemOrderedEvent? ItemOrderedEvent { get; set; }

    public Guid? ItemSoldEventId { get; set; }
    public virtual ItemSoldEvent? ItemSoldEvent { get; set; }
}
