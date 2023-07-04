namespace Resourcerer.DataAccess.Entities;

public class ElementDeliveredEvent : EntityBase
{
    public Guid ElementPurchasedEventId { get; set; }
    public virtual ElementPurchasedEvent? ElementPurchasedEvent { get; set; }
}
