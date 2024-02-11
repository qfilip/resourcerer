using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace Resourcerer.DataAccess.Entities;

public class Instance : EntityBase
{
    public int PurchaseDiscountPercent { get; set; }
    public double UnitPurchasePrice { get; set; }
    public double Quantity { get; set; }
    public DateTime? ExpiryDate { get; set; }
    
    public Guid? ItemId { get; set; }
    public virtual Item? Item { get; set; }

    public Guid OwnerCompanyId { get; set; }
    public Company? OwnerCompany { get; set; }

    public Guid? SellerCompanyId { get; set; }
    public Company? SellerCompany { get; set; }

    public string OrderedEventsJson
    {
        get => JsonSerializer.Serialize(OrderedEvents);
        set
        {
            if(value == null)
            {
                return;
            }

            OrderedEvents = JsonSerializer.Deserialize<List<InstanceOrderedEvent>>(value)!;
        }
    }

    public string DiscardedEventsJson
    {
        get => JsonSerializer.Serialize(DiscardedEvents);
        set
        {
            if (value == null)
            {
                return;
            }

            DiscardedEvents = JsonSerializer.Deserialize<List<InstanceDiscardedEvent>>(value)!;
        }
    }

    [NotMapped]
    public List<InstanceOrderedEvent> OrderedEvents { get; set; } = new();
    [NotMapped]
    public List<InstanceDiscardedEvent> DiscardedEvents { get; set; } = new();
}
