namespace Resourcerer.Dtos.V1;

public class V1InstanceOrderCancelCommand : V1InstanceOrderCommand
{
    public Guid InstanceId { get; set; }
    public Guid OrderEventId { get; set; }
    public string? Reason { get; set; }
    public decimal RefundedAmount { get; set; }
}
