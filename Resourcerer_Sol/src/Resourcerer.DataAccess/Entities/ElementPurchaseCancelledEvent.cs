namespace Resourcerer.DataAccess.Entities;

public class ElementPurchaseCancelledEvent : EntityBase
{
    public Guid ElementPurchasedEventId { get; set; }
    public virtual ElementPurchasedEvent? ElementPurchasedEvent { get; set; }
}
