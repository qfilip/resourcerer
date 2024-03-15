namespace Resourcerer.Dtos.Instances.Events.Order;

public class InstanceOrderCancelRequestDto : InstanceOrderEventDtoBase
{
    public Guid InstanceId { get; set; }
    public string? OrderEventId { get; set; }
    public string? Reason { get; set; }
    public double RefundedAmount { get; set; }
}
