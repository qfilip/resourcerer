using Resourcerer.Dtos.Instances.Events.Order;

namespace Resourcerer.Dtos.Instances.Events;

public class InstanceDiscardedRequestDto : InstanceOrderEventDtoBase
{
    public Guid InstanceId { get; set; }
    public double Quantity { get; set; }
    public string? Reason { get; set; }
}
