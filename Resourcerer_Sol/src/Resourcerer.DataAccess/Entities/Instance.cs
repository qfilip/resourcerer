namespace Resourcerer.DataAccess.Entities;

public class Instance : EntityBase
{
    public Instance()
    {
        InstanceSellRequestedEvents = new HashSet<InstanceSellRequestedEvent>();
        InstanceDiscardedEvents = new HashSet<InstanceDiscardedEvent>();
    }
    public double Quantity { get; set; }
    public double UnitPrice { get; set; }
    public int TotalDiscountPercent { get; set; }
    public DateTime? ExpectedDeliveryDate { get; set; }
    public DateTime? ExpiryDate { get; set; }

    public Guid? ItemId { get; set; }
    public virtual Item? Item { get; set; }

    public Guid InstanceBuyRequestedEventId { get; set; }
    public virtual InstanceBuyRequestedEvent? InstanceBuyRequestedEvent { get; set; }
    
    public ICollection<InstanceSellRequestedEvent> InstanceSellRequestedEvents { get; set; }
    public ICollection<InstanceDiscardedEvent> InstanceDiscardedEvents { get; set; }    
}
