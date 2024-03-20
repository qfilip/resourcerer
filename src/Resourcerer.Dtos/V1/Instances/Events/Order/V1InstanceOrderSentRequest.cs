namespace Resourcerer.Dtos.V1;

public class V1InstanceOrderSentRequest : V1InstanceOrderEvent
{
    public Guid InstanceId { get; set; }
    public Guid OrderEventId { get; set; }
}
