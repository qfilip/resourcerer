namespace Resourcerer.Dtos;

public class InstanceDto
{
    public double Quantity { get; set; }
    public DateTime? ExpiryDate { get; set; }

    public Guid? ElementId { get; set; }
    public virtual ElementDto? Element { get; set; }
    public Guid? CompositeId { get; set; }
    public virtual CompositeDto? Composite { get; set; }

    public List<InstanceOrderedEventDto>? InstanceOrderedEvents { get; set; }
    public List<InstanceOrderCancelledEventDto>? InstanceOrderCancelledEvents { get; set; }
    public List<InstanceDeliveredEventDto>? InstanceDeliveredEvents { get; set; }
    public List<InstanceDiscardedEventDto>? InstanceDiscardedEvents { get; set; }
}
