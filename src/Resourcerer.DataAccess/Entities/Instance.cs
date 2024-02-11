using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace Resourcerer.DataAccess.Entities;

public class Instance : EntityBase
{   
    public DateTime? ExpiryDate { get; set; }
    
    public Guid? ItemId { get; set; }
    public virtual Item? Item { get; set; }

    public Guid OwnerCompanyId { get; set; }
    public Company? Company { get; set; }

    public string InstanceOrderedEventsJson
    {
        get => JsonSerializer.Serialize(InstanceOrderedEvents);
        set
        {
            if(value == null)
            {
                return;
            }

            InstanceOrderedEvents = JsonSerializer.Deserialize<List<InstanceOrderedEvent>>(value)!;
        }
    }

    public string InstanceDiscardedEventsJson
    {
        get => JsonSerializer.Serialize(InstanceDiscardedEvents);
        set
        {
            if (value == null)
            {
                return;
            }

            InstanceDiscardedEvents = JsonSerializer.Deserialize<List<InstanceDiscardedEvent>>(value)!;
        }
    }

    [NotMapped]
    public List<InstanceOrderedEvent> InstanceOrderedEvents { get; set; } = new();
    [NotMapped]
    public List<InstanceDiscardedEvent> InstanceDiscardedEvents { get; set; } = new();
}
