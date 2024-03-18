using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace Resourcerer.DataAccess.Entities;

public class Instance : EntityBase
{
    public Instance()
    {
        DerivedInstances = new HashSet<Instance>();
    }

    public double Quantity { get; set; }
    public DateTime? ExpiryDate { get; set; }
    
    public Guid? ItemId { get; set; }
    public virtual Item? Item { get; set; }

    public Guid OwnerCompanyId { get; set; }
    public virtual Company? OwnerCompany { get; set; }

    public Guid? SourceInstanceId { get; set; }
    public virtual Instance? SourceInstance { get; set; }

    public ICollection<Instance> DerivedInstances { get; set; }

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
    public string ReservedEventsJson
    {
        get => JsonSerializer.Serialize(ReservedEvents);
        set
        {
            if (value == null)
            {
                return;
            }

            ReservedEvents = JsonSerializer.Deserialize<List<InstanceReservedEvent>>(value)!;
        }
    }

    [NotMapped]
    public List<InstanceOrderedEvent> OrderedEvents { get; set; } = new();
    [NotMapped]
    public List<InstanceDiscardedEvent> DiscardedEvents { get; set; } = new();
    [NotMapped]
    public List<InstanceReservedEvent> ReservedEvents { get; set; } = new();
}
