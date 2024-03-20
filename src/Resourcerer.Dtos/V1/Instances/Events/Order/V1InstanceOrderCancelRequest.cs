namespace Resourcerer.Dtos.V1;

public class V1InstanceOrderCancelRequest : V1InstanceOrderEvent
{
    public Guid InstanceId { get; set; }
    public string? OrderEventId { get; set; }
    public string? Reason { get; set; }
    public double RefundedAmount { get; set; }
}
