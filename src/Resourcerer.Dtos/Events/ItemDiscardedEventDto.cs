using Resourcerer.Dtos.Events;

namespace Resourcerer.Dtos;

public class ItemDiscardedEventDto : InstanceEventDtoBase
{
    public string? Owner { get; set; }
    public double Quantity { get; set; }
    public string? Reason { get; set; }

    public Guid InstanceId { get; set; }
    public virtual InstanceDto? Instance { get; set; }
}
