namespace Resourcerer.Dtos.V1;

public class V1InstanceOrderDeliveredRequest : V1InstanceOrderEvent
{
    public Guid InstanceId { get; set; }
    public Guid OrderEventId { get; set; }
}
