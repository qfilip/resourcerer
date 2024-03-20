namespace Resourcerer.Dtos.V1;

public class V1InstanceOrderCancelRequest : V1InstanceOrderEvent
{
    public Guid InstanceId { get; set; }
    public Guid OrderEventId { get; set; }
    public string? Reason { get; set; }
    public double RefundedAmount { get; set; }
}
