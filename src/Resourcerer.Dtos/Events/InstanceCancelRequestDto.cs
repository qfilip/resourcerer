using Resourcerer.Dtos.Events;

namespace Resourcerer.Dtos;

public class InstanceCancelRequestDto : InstanceEventDtoBase
{
    public Guid InstanceId { get; set; }
    public Guid OrderEventId { get; set; }
    public string? Reason { get; set; }
    public double RefundedAmount { get; set; }
}
