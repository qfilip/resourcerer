using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace Resourcerer.DataAccess.Entities;

public class Instance : AppDbEntity
{
    public Instance()
    {
        DerivedInstances = new HashSet<Instance>();
        OrderedEvents = new HashSet<InstanceOrderedEvent>();
        ReservedEvents = new HashSet<InstanceReservedEvent>();
        DiscardedEvents = new HashSet<InstanceDiscardedEvent>();
    }

    public double Quantity { get; set; }
    public DateTime? ExpiryDate { get; set; }
    
    // relational
    public Guid? ItemId { get; set; }
    public virtual Item? Item { get; set; }

    public Guid OwnerCompanyId { get; set; }
    public virtual Company? OwnerCompany { get; set; }

    public Guid? SourceInstanceId { get; set; }
    public virtual Instance? SourceInstance { get; set; }

    public ICollection<Instance> DerivedInstances { get; set; }

    public ICollection<InstanceOrderedEvent> OrderedEvents { get; set; }
    public ICollection<InstanceReservedEvent> ReservedEvents { get; set; }
    public ICollection<InstanceDiscardedEvent> DiscardedEvents { get; set; }
}
