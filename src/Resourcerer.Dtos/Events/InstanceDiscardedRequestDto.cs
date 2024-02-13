using Resourcerer.Dtos.Events;

namespace Resourcerer.Dtos;

public class InstanceDiscardedRequestDto : EventDtoBase
{
    public Guid InstanceId { get; set; }
    public double Quantity { get; set; }
    public string? Reason { get; set; }
}
