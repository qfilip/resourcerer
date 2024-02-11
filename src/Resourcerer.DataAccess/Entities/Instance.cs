using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace Resourcerer.DataAccess.Entities;

public class Instance : EntityBase
{   
    public DateTime? ExpiryDate { get; set; }
    public Guid? ItemId { get; set; }
    public virtual Item? Item { get; set; }

    public string ItemOrderedEventsJson
    {
        get => JsonSerializer.Serialize(ItemOrderedEvents);
        set
        {
            if(value == null)
            {
                return;
            }

            ItemOrderedEvents = JsonSerializer.Deserialize<List<InstanceOrderedEvent>>(value)!;
        }
    }

    public string ItemDiscardedEventsJson
    {
        get => JsonSerializer.Serialize(ItemDiscardedEvents);
        set
        {
            if (value == null)
            {
                return;
            }

            ItemDiscardedEvents = JsonSerializer.Deserialize<List<InstanceDiscardedEvent>>(value)!;
        }
    }

    [NotMapped]
    public List<InstanceOrderedEvent> ItemOrderedEvents { get; set; } = new();
    [NotMapped]
    public List<InstanceDiscardedEvent> ItemDiscardedEvents { get; set; } = new();
}
